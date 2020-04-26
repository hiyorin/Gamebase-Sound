#if GAMEBASE_ADD_MASTERAUDIO
using System.Collections.Generic;
using System.Linq;
using DarkTonic.MasterAudio;
using Gamebase.Loader.Asset;
using JetBrains.Annotations;
using UniRx.Async;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using MA = DarkTonic.MasterAudio.MasterAudio;

namespace Gamebase.Sound.MasterAudio.Internal
{
    [PublicAPI]
    internal sealed class MasterAudioManager : ISoundManager
    {
        private readonly MA masterAudioPrefab;
        
        private readonly MasterAudioPlayer.Pool playerFactory;
        
        private readonly MasterAudioSettings settings;
        
        private readonly IAssetLoader assetLoader;
        
        private readonly Dictionary<ISoundPlayer, AsyncOperationHandle> soundPacks = new Dictionary<ISoundPlayer, AsyncOperationHandle>();
        
        public MasterAudioManager(
            MA masterAudioPrefab,
            MasterAudioPlayer.Pool playerFactory,
            MasterAudioSettings settings,
            IAssetLoader assetLoader)
        {
            this.masterAudioPrefab = masterAudioPrefab;
            this.playerFactory = playerFactory;
            this.settings = settings;
            this.assetLoader = assetLoader;
        }
        
        #region ISoundManager implementation

        async UniTask ISoundManager.Initialize()
        {
            if (MA.SafeInstance == null)
            {
                Object.Instantiate(masterAudioPrefab);
            }
        }

        void ISoundManager.Dispose()
        {
            foreach (var sound in soundPacks.Values)
            {
                assetLoader.Unload(sound);
            }
            soundPacks.Clear();

            Object.Destroy(MA.SafeInstance);
        }

        async UniTask<ISoundPlayer> ISoundManager.Load(string path)
        {
            var handle = await assetLoader.Load<GameObject>(path);
            var sound = handle.Result.GetComponent<DynamicSoundGroupCreator>();
            var player = playerFactory.Spawn(path, sound, settings.Bgm.AudioMixerGroup);
            soundPacks.Add(player, handle);
            return player;
        }

        void ISoundManager.Unload(ISoundPlayer player)
        {
            if (!soundPacks.TryGetValue(player, out var sound))
            {
                Debug.unityLogger.LogError(nameof(MasterAudioManager), $"{player.FileName} is not found.");
                return;
            }
            
            assetLoader.Unload(sound);
            soundPacks.Remove(player);
        }

        IEnumerable<string> ISoundManager.GetInfos() => soundPacks.Keys.Select(x => x.FileName);

        #endregion
    }
}
#endif