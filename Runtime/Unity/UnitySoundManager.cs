using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UniRx.Async;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using Zenject;
using Object = UnityEngine.Object;

namespace Gamebase.Sound.Unity
{
    public sealed class UnitySoundManager : ISoundManager, IInitializable, IDisposable
    {
        private sealed class SoundData
        {
            public UnitySoundPack Pack;
            public int Count;
            public readonly List<string> SoundNames = new List<string>();
        }

        [Inject] private UnitySoundSettings settings = null;
        
        [Inject] private UnitySoundPack[] preloadPacks = null;

        // [Inject] private ISoundConfigUseCase soundConfigUseCase = null;
        
        private GameObject container;

        private UnitySoundSource bgmSource, seSource, voiceSource;

        private bool initialized = false;
        
        private readonly Dictionary<string, SoundData> packs = new Dictionary<string, SoundData>();
        
        void IInitializable.Initialize()
        {
            // Create root container.
            container = new GameObject(nameof(UnitySoundManager));
            container.transform.SetParent(ProjectContext.Instance.transform, false);

            // Create sound source
            bgmSource = CreateSoundSource("BGM", container.transform,
                settings.Bgm.AudioMixerGroup,
                settings.Bgm.PlaybackNumber);
            seSource = CreateSoundSource("SE", container.transform,
                settings.Se.AudioMixerGroup,
                settings.Se.PlaybackNumber);
            voiceSource = CreateSoundSource("Voice", container.transform,
                settings.Voice.AudioMixerGroup,
                settings.Voice.PlaybackNumber);
            
            // Preload Packs
            if (preloadPacks != null)
            {
                foreach (var pack in preloadPacks)
                {
                    var reference = new SoundData()
                    {
                        Pack = pack,
                        Count = 1,
                    };
                    packs.Add(pack.name, reference);
                }
            }

            // Initialize
            Initialize().GetAwaiter();
        }

        void IDisposable.Dispose()
        {
            if (container != null)
            {
                Object.Destroy(container);
                container = null;
            }
        }

        private async UniTask Initialize()
        {
            // await soundConfigUseCase.Initialize();
            initialized = true;
        }

        private async UniTask<SoundData> LoadInternal(string fileName)
        {
            if (!packs.TryGetValue(fileName, out var reference))
            {
                var operation = Addressables.LoadAssetAsync<UnitySoundPack>(fileName);
                await operation.Task;
                if (!operation.IsValid())
                    throw operation.OperationException;

                reference = new SoundData()
                {
                    Pack = operation.Result,
                    Count = 0,
                };
                
                reference.SoundNames.AddRange(reference.Pack.BgmClips.Select(x => x.name));
                reference.SoundNames.AddRange(reference.Pack.SeClips.Select(x => x.name));
                reference.SoundNames.AddRange(reference.Pack.VoiceClips.Select(x => x.name));
                
                packs.Add(fileName, reference);
            }
            
            // 参照カウント
            reference.Count++;

            return reference;
        }
        
        private static UnitySoundSource CreateSoundSource(string name, Transform parent, AudioMixerGroup group, int playbackNumber)
        {
            var gameObject = new GameObject(name);
            gameObject.transform.SetParent(parent, false);
            
            var source = gameObject.AddComponent<UnitySoundSource>();
            source.Initialize(playbackNumber, group);

            return source;
        }
        
        #region ISoundManager implementation
        
        public async UniTask<ISoundPlayer> Load(string fileName)
        {
            // Wait initialization
            await UniTask.WaitUntil(() => initialized);
            
            SoundData reference = await LoadInternal(fileName);
            
            var player = container.AddComponent<UnitySoundPlayer>();
            player.Initialize(fileName, reference.Pack, reference.SoundNames, bgmSource, seSource, voiceSource);
            
            return player;
        }

        public bool Unload(ISoundPlayer player)
        {
            return PreUnload(player.FileName);
        }

        public async UniTask<IList<string>> PreLoad(string fileName)
        {
            // Wait initialization
            await UniTask.WaitUntil(() => initialized);
            
            SoundData reference = await LoadInternal(fileName);
            if (reference == null)
                throw new ArgumentNullException();

            return reference.SoundNames;
        }

        public bool PreUnload(string fileName)
        {
            if (!packs.TryGetValue(fileName, out var reference))
            {
                Debug.unityLogger.LogError(nameof(UnitySoundManager), $"{fileName} is not found.");
                return false;
            }
            
            if (--reference.Count <= 0)
            {
                Addressables.Release(reference.Pack);
                packs.Remove(fileName);
            }
            
            return true;
        }

        public void GetCacheInfos(ref IList<string> dst)
        {
            if (dst == null)
                return;

            foreach (var reference in packs)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine($"{reference.Key}");

                foreach (var clip in reference.Value.Pack.BgmClips)
                    builder.AppendLine($"BGM       : {clip.name}");
                
                foreach (var clip in reference.Value.Pack.SeClips)
                    builder.AppendLine($"SE        : {clip.name}");

                foreach (var clip in reference.Value.Pack.VoiceClips)
                    builder.AppendLine($"Voice     : {clip.name}");

                builder.AppendLine($"ref count : {reference.Value.Count}");
                dst.Add(builder.ToString());
            }
        }
        
        #endregion
    }
}