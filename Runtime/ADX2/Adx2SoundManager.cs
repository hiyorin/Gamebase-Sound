#if GAMEBASE_ADD_ADX2
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gamebase.Application.IO;
using Gamebase.Application.Sound;
using Gamebase.Domain.UseCase;
using UniRx.Async;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Gamebase.Sound.Adx2
{   
    public sealed class Adx2SoundManager : IInitializable, IDisposable, ISoundManager
    {
        private class CueSheetData
        {
            public CriAtomCueSheet CueSheet;
            public int Count;
        }
        
        [Inject] private CriWareInitializer criWareInitializerPrefab = null;

        [Inject] private Adx2Settings settings = null;

        [Inject] private ISoundConfigUseCase soundConfigUseCase = null;
        
        private GameObject container;

        private bool isInitialized = false;
        
        private readonly Dictionary<string, CueSheetData> cueSheets = new Dictionary<string, CueSheetData>();

        private readonly List<Adx2SoundPlayer> players = new List<Adx2SoundPlayer>();
        
        private readonly CacheDirectory cache = new CacheDirectory("Sound");
        
        void IInitializable.Initialize()
        {
            // 1. ProjectContextにCriAtomを置く
            container = new GameObject(nameof(Adx2SoundManager));
            container.transform.SetParent(ProjectContext.Instance.transform, false);
            
            // 2. Create CriAtomServer
            CriWare.managerObject.transform.SetParent(container.transform, false);
            
            // 3. Create CriAtom
            var criAtom = new GameObject(nameof(CriAtom))
                .AddComponent<CriAtom>();
            criAtom.transform.SetParent(container.transform, false);

            // 4. Create CriWareErrorHandler
            var criWareErrorHandler = new GameObject(nameof(CriWareErrorHandler))
                .AddComponent<CriWareErrorHandler>();
            criWareErrorHandler.transform.SetParent(container.transform, false);
            criWareErrorHandler.enableDebugPrintOnTerminal = true;
            
            // 5. Create CriAtomInitializer
            var initializer = Object.Instantiate(criWareInitializerPrefab);
            initializer.transform.SetParent(container.transform, false);
            initializer.Initialize();

            Initialize().GetAwaiter();
        }

        void IDisposable.Dispose()
        {
            if (CriWareInitializer.IsInitialized())
            {
                foreach (var criAtomCueSheet in cueSheets)
                {
                    CriAtom.RemoveCueSheet(criAtomCueSheet.Key);
                }

                cueSheets.Clear();

                CriAtomEx.UnregisterAcf();
            }

            if (container != null)
            {
                Object.Destroy(container);
                container = null;
            }

            isInitialized = false;
        }

        private async UniTask Initialize()
        {
            if (!settings.DefaultAcfReference.RuntimeKeyIsValid())
                return;

            // Wait sound config initialization
            await soundConfigUseCase.Initialize();
            
            // Wait CriWare initialization
            await UniTask.WaitUntil(CriWareInitializer.IsInitialized);
            await LoadAcfFile();

            // プリロードするサウンドファイルをロード
            var reference = settings.PreloadCueSheetReference;
            if (!string.IsNullOrEmpty(reference.CueSheetName) || reference.AcbReference.RuntimeKeyIsValid())
            {
                Adx2SoundLoader loader = new Adx2SoundLoader(cache);
                CriAtomCueSheet cueSheet = await loader.Load(
                    reference.CueSheetName,
                    reference.AcbReference.RuntimeKey,
                    reference.AwbReference.RuntimeKeyIsValid() ?
                        reference.AwbReference.RuntimeKey :
                        default(object));
                
                CueSheetData data = new CueSheetData()
                {
                    CueSheet =  cueSheet,
                    Count = 1,
                };
                cueSheets.Add(settings.PreloadCueSheetReference.CueSheetName, data);
            }
            
            isInitialized = true;
        }

        private byte[] acfBytes;
        
        private async UniTask LoadAcfFile()
        {
            var operation = settings.DefaultAcfReference.LoadAssetAsync<TextAsset>();
            await operation.Task;
            if (!operation.IsValid())
                throw operation.OperationException;
            
            // メモリ上に存在し続けないといけない
            acfBytes = operation.Result.bytes;
            CriAtomEx.RegisterAcf(acfBytes);

            settings.DefaultAcfReference.ReleaseAsset();
        }
        
        #region ISoundManager implemantation

        public async UniTask<ISoundPlayer> Load(string fileName)
        {
            await UniTask.WaitUntil(() => isInitialized);

            var cueNames = await PreLoad(fileName);
            
            string cueSheetName = Path.GetFileNameWithoutExtension(fileName);
            Adx2SoundPlayer player = container.AddComponent<Adx2SoundPlayer>();
            player.Initialize(cueSheetName, cueNames);
            players.Add(player);
            
            return player;
        }

        public bool Unload(ISoundPlayer player)
        {
            if (!isInitialized)
                return false;
            
            var target = players.FirstOrDefault(x => x.Equals(player));
            if (target == null)
                return false;

            players.Remove(target);
            Object.Destroy(target);

            PreUnload(player.FileName);
            
            return true;
        }

        public async UniTask<IList<string>> PreLoad(string fileName)
        {
            await UniTask.WaitUntil(() => isInitialized);
         
            string directory = Path.GetDirectoryName(fileName);
            string cueSheetName = Path.GetFileNameWithoutExtension(fileName);
            string acbFileName = $"{directory}/{cueSheetName}.acb";
            string awbFileName = $"{directory}/{cueSheetName}.awb";
            
            CueSheetData reference;
            if (!cueSheets.TryGetValue(cueSheetName, out reference))
            {
                Adx2SoundLoader loader = new Adx2SoundLoader(cache);
                CriAtomCueSheet cueSheet = await loader.Load(cueSheetName, acbFileName, awbFileName);
                if (cueSheet.loaderStatus == CriAtomExAcbLoader.Status.Error)
                {
                    Debug.unityLogger.LogError(nameof(Adx2SoundManager), $"Failed to load {cueSheetName}.");
                    return null;
                }

                reference = new CueSheetData {CueSheet = cueSheet, Count = 0};
                cueSheets.Add(cueSheetName, reference);
            }

            // 参照カウント
            reference.Count++;

            // 収録されているCueName一覧を作る
            IList<string> cueNames = new List<string>();
            CriAtomExAcb acb = CriAtom.GetAcb(cueSheetName);
            foreach (var cueInfo in acb.GetCueInfoList())
            {
                cueNames.Add(cueInfo.name);
            }
            
            return cueNames;
        }

        public bool PreUnload(string fileName)
        {
            if (!isInitialized)
                return false;

            CueSheetData reference;
            if (!cueSheets.TryGetValue(fileName, out reference))
            {
                Debug.unityLogger.LogError(nameof(Adx2SoundManager), $"{fileName} is not found.");
                return false;
            }

            if (--reference.Count <= 0)
            {
                CriAtom.RemoveCueSheet(fileName);
                cueSheets.Remove(fileName);
            }

            return true;
        }
        
        public void GetCacheInfos(ref IList<string> dst)
        {
            if (dst == null)
                return;

            foreach (var reference in cueSheets)
            {
                StringBuilder builder = new StringBuilder();
                builder
                    .AppendLine($"{reference.Key}")
                    .AppendLine($"acb       : {reference.Value.CueSheet.acbFile}")
                    .AppendLine($"awb       : {reference.Value.CueSheet.awbFile}")
                    .AppendLine($"ref count : {reference.Value.Count}");
                dst.Add(builder.ToString());
            }
        }
        
        #endregion
    }
}
#endif