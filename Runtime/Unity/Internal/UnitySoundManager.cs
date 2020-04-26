using System.Collections.Generic;
using Gamebase.Loader.Asset;
using JetBrains.Annotations;
using UniRx.Async;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Gamebase.Sound.Unity.Internal
{
    [PublicAPI]
    internal sealed class UnitySoundManager : ISoundManager
    {
        private readonly UnitySoundSource.Pool soundSourceFactory;

        private readonly UnitySoundPlayer.Pool soundPlayerFactory;

        private readonly UnitySoundSettings settings;

        private readonly IAssetLoader assetLoader;
        
        private UnitySoundSource bgmSource, seSource, voiceSource;

        private readonly Dictionary<ISoundPlayer, AsyncOperationHandle> soundPacks = new Dictionary<ISoundPlayer, AsyncOperationHandle>();
        
        public UnitySoundManager(
            UnitySoundSource.Pool soundSourceFactory,
            UnitySoundPlayer.Pool soundPlayerFactory,
            UnitySoundSettings settings,
            IAssetLoader assetLoader)
        {
            this.soundSourceFactory = soundSourceFactory;
            this.soundPlayerFactory = soundPlayerFactory;
            this.settings = settings;
            this.assetLoader = assetLoader;
        }

        private UnitySoundSource CreateSoundSource(UnitySoundSettings.Category category)
        {
            return soundSourceFactory.Spawn(category.PlaybackNumber, category.AudioMixerGroup);
        }

        #region ISoundManager implementation

        async UniTask ISoundManager.Initialize()
        {
            bgmSource = CreateSoundSource(settings.Bgm);
            seSource = CreateSoundSource(settings.Se);
            voiceSource = CreateSoundSource(settings.Voice);
        }

        void ISoundManager.Dispose()
        {
            soundSourceFactory.Despawn(bgmSource);
            soundSourceFactory.Despawn(seSource);
            soundSourceFactory.Despawn(voiceSource);
        }

        async UniTask<ISoundPlayer> ISoundManager.Load(string path)
        {
            var handle = await assetLoader.Load<UnitySoundPack>(path);
            var player = soundPlayerFactory.Spawn(handle.Result, bgmSource, seSource, voiceSource);
            soundPacks.Add(player, handle);
            return player;
        }

        void ISoundManager.Unload(ISoundPlayer player)
        {
            if (soundPacks.TryGetValue(player, out var handle))
            {
                soundPacks.Remove(player);
                assetLoader.Unload(handle);
            }
        }
        
        #endregion
    }
}