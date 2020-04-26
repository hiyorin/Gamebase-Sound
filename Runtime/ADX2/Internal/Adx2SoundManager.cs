#if GAMEBASE_ADD_ADX2
using System.Collections.Generic;
using Gamebase.Loader.Asset;
using Gamebase.Utilities;
using JetBrains.Annotations;
using UniRx.Async;
using UnityEngine;

namespace Gamebase.Sound.Adx2.Internal
{
    [PublicAPI]
    internal sealed class Adx2SoundManager : ISoundManager
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

        private readonly CacheDirectory cacheDirectory = new CacheDirectory("Sound");
        
        private readonly Dictionary<string, CueSheetData> cueSheets = new Dictionary<string, CueSheetData>();

        private readonly List<Adx2SoundPlayer> players = new List<Adx2SoundPlayer>();

        private readonly Adx2SoundLoader soundLoader;
        
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
            
            soundLoader = new Adx2SoundLoader(assetLoader, cacheDirectory);
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
            
            foreach (var cueSheetData in cueSheets.Values)
            {
                soundLoader.Unload(cueSheetData.CueSheet);
            }
            cueSheets.Clear();

            Object.DestroyImmediate(criAtom);
        }

        async UniTask<ISoundPlayer> ISoundManager.Load(string path)
        {
            if (!cueSheets.TryGetValue(path, out var reference))
            {
                var cueSheet = await soundLoader.Load(path);
                if (cueSheet.loaderStatus == CriAtomExAcbLoader.Status.Error)
                {
                    Debug.unityLogger.LogError(nameof(Adx2SoundManager), $"Failed to load {path}.");
                    return null;
                }
                
                reference = new CueSheetData {CueSheet = cueSheet, Count = 0};
                cueSheets.Add(path, reference);
            }
            
            reference.Count++;

            var player = playerFactory.Spawn(reference.CueSheet.name);
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
#endif