#if GAMEBASE_ADD_MASTERAUDIO
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DarkTonic.MasterAudio;
using UniRx.Async;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using Sound = DarkTonic.MasterAudio.MasterAudio;

namespace Gamebase.Sound.MasterAudio
{
    internal sealed class MasterAudioManager : IInitializable, ISoundManager
    {
        private sealed class Cache
        {
            public AsyncOperationHandle Handle;
            public DynamicSoundGroupCreator Instance;
            public MasterAudioPlayer Player;
            public int RefCount;
        }

        [Inject] private Sound masterAudio = default;

        [Inject] private MasterAudioSettings settings = default;

        private MasterAudioPlayer playerPrefab;
        
        private readonly Dictionary<string, Cache> caches = new Dictionary<string, Cache>();

        public void Initialize()
        {
            // Initialize MasterAudio
            if (Sound.SafeInstance == null)
            {
                Object.Instantiate(masterAudio);
            }

            playerPrefab = Resources.Load<MasterAudioPlayer>("PlaylistController");
        }

        private async UniTask<Cache> LoadInternal(string fileName)
        {
            if (!caches.TryGetValue(fileName, out var cache))
            {
                var operation = Addressables.LoadAssetAsync<GameObject>(fileName);
                await operation.Task;

                if (!operation.IsValid())
                {
                    Debug.unityLogger.LogError(nameof(MasterAudioManager), "Load Error.");
                    throw operation.OperationException;
                }

                var asset = operation.Result.GetComponent<DynamicSoundGroupCreator>();
                var instance = Object.Instantiate(asset);
                var player = Object.Instantiate(playerPrefab, instance.transform);
                player.Initialize(fileName, instance, settings.Bgm.AudioMixerGroup);
                
                cache = new Cache()
                {
                    Handle = operation,
                    Instance = instance,
                    Player = player,
                    RefCount = 0,
                };

                caches.Add(fileName, cache);
            }

            cache.RefCount++;
            
            return cache;
        }
        
        private bool UnloadInternal(string fileName)
        {
            if (!caches.TryGetValue(fileName, out var reference))
            {
                Debug.unityLogger.LogError(nameof(MasterAudioManager), $"{fileName} is not found.");
                return false;
            }
            
            if (--reference.RefCount <= 0)
            {
                reference.Player.Dispose();
                Object.Destroy(reference.Instance.gameObject);
                Addressables.Release(reference.Handle);
                caches.Remove(fileName);
            }

            return true;
        }
        
        #region ISoundManager implementation
        
        public async UniTask<ISoundPlayer> Load(string fileName)
        {
            var cache = await LoadInternal(fileName);
            return cache.Player;
        }

        public bool Unload(ISoundPlayer player)
        {
            if (player == null)
                return false;

            return UnloadInternal(player.FileName);
        }

        public async UniTask<IList<string>> PreLoad(string fileName)
        {
            var cache = await LoadInternal(fileName);
            return cache.Player.SoundNames.ToList();
        }

        public bool PreUnload(string fileName)
        {
            return UnloadInternal(fileName);
        }

        public void GetCacheInfos(ref IList<string> dst)
        {
            if (dst == null)
                return;

            foreach (var reference in caches)
            {
                var builder = new StringBuilder();
                builder.AppendLine($"{reference.Key}");

                foreach (var playerSoundName in reference.Value.Player.SoundNames)
                {
                    builder.AppendLine($"  {playerSoundName}");
                }
                
                builder.AppendLine($"  Reference Count : {reference.Value.RefCount}");
                dst.Add(builder.ToString());
            }
        }
        
        #endregion
    }
}
#endif