using System.Collections.Generic;
using System.IO;
using Gamebase.Loader.Asset;
using Gamebase.Utilities;
using JetBrains.Annotations;
using UniRx.Async;
using UnityEngine;

namespace Gamebase.Sound.Adx2
{
    [PublicAPI]
    public sealed class Adx2SoundManager : ISoundManager
    {
        private class CueSheetData
        {
            public CriAtomCueSheet CueSheet;
            public int Count;
        }
        
        private readonly CriAtom criAtomPrefab;
        
        private readonly CriWareInitializer criWareInitializerPrefab;

        private readonly Adx2SoundPlayer.Pool playerFactory;
        
        private readonly Adx2Settings settings;
        
        private readonly IAssetLoader assetLoader;

        private readonly CacheDirectory cacheDirectory = new CacheDirectory("Sound");
        
        private readonly Dictionary<string, CueSheetData> cueSheets = new Dictionary<string, CueSheetData>();

        private readonly List<Adx2SoundPlayer> players = new List<Adx2SoundPlayer>();
        
        private CriAtom criAtom;

        private byte[] acfBytes;
        
        public Adx2SoundManager(
            CriAtom criAtomPrefab,
            CriWareInitializer criWareInitializerPrefab,
            Adx2SoundPlayer.Pool playerFactory,
            Adx2Settings settings,
            IAssetLoader assetLoader)
        {
            this.criAtomPrefab = criAtomPrefab;
            this.criWareInitializerPrefab = criWareInitializerPrefab;
            this.playerFactory = playerFactory;
            this.settings = settings;
            this.assetLoader = assetLoader;
        }
        
        private async UniTask LoadAcfFile(AcfAssetReference reference)
        {
            var operation = reference.LoadAssetAsync<TextAsset>();
            await operation.Task;
            if (!operation.IsValid())
                throw operation.OperationException;
            
            // メモリ上に存在し続けないといけない
            acfBytes = operation.Result.bytes;
            CriAtomEx.RegisterAcf(acfBytes);

            reference.ReleaseAsset();
        }
        
        #region ISoundManager implementation

        async UniTask ISoundManager.Initialize()
        {
            criAtom = Object.Instantiate(criAtomPrefab);
            
            var initializer = Object.Instantiate(criWareInitializerPrefab);
            initializer.Initialize();
            await UniTask.WaitUntil(CriWareInitializer.IsInitialized);
            Object.DestroyImmediate(initializer);

            await LoadAcfFile(settings.DefaultAcfReference);
        }

        void ISoundManager.Dispose()
        {
            foreach (var player in players)
            {
                playerFactory.Despawn(player);
            }
            players.Clear();
            
            foreach (var cueSheet in cueSheets.Keys)
            {
                CriAtom.RemoveCueSheet(cueSheet);
            }
            cueSheets.Clear();

            Object.DestroyImmediate(criAtom);
        }

        async UniTask<ISoundPlayer> ISoundManager.Load(string path)
        {
            var directory = Path.GetDirectoryName(path);
            var cueSheetName = Path.GetFileNameWithoutExtension(path);
            var acbFileName = $"{cueSheetName}.acb";
            var awbFileName = $"{cueSheetName}.awb";
            
            if (!cueSheets.TryGetValue(cueSheetName, out var reference))
            {
                var loader = new Adx2SoundLoader(assetLoader, cacheDirectory);
                var cueSheet = await loader.Load(cueSheetName, acbFileName, awbFileName);
                if (cueSheet.loaderStatus == CriAtomExAcbLoader.Status.Error)
                {
                    Debug.unityLogger.LogError(nameof(Adx2SoundManager), $"Failed to load {cueSheetName}.");
                    return null;
                }
                
                reference = new CueSheetData {CueSheet = cueSheet, Count = 0};
                cueSheets.Add(cueSheetName, reference);
            }
            
            reference.Count++;

            var player = playerFactory.Spawn(cueSheetName);
            players.Add(player);
            return player;
        }

        void ISoundManager.Unload(ISoundPlayer player)
        {
            if (!players.Contains(player as Adx2SoundPlayer))
                return;

            if (!cueSheets.TryGetValue(player.FileName, out var cueSheet))
            {
                Debug.unityLogger.LogError(nameof(Adx2SoundManager), $"{player.FileName} is not found.");
                return;
            }
            
            if (--cueSheet.Count <= 0)
            {
                CriAtom.RemoveCueSheet(player.FileName);
                cueSheets.Remove(player.FileName);
            }

            players.Remove(player as Adx2SoundPlayer);
            playerFactory.Despawn(player as Adx2SoundPlayer);
        }
        
        #endregion
    }
}