using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Gamebase.Sound.Unity
{
    public sealed class UnitySoundSource : MonoBehaviour
    {
        private readonly List<AudioSource> sources = new List<AudioSource>();
        
        private readonly HashSet<AudioClip> playQueue = new HashSet<AudioClip>();

        private void Update()
        {
            // Play
            foreach (var clip in playQueue)
            {
                if (!PlayInternal(clip, false))
                    break;
            }
            playQueue.Clear();

            foreach (var source in sources)
            {
                if (!source.enabled)
                    continue;

                if (source.time >= source.clip.length)
                    source.enabled = false;
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void Initialize(int playbackNumber, AudioMixerGroup group)
        {
            for (var i = 0; i < playbackNumber; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.enabled = false;
                source.outputAudioMixerGroup = group;
                
                sources.Add(source);
            }
        }

        private void Dispose()
        {
            foreach (var audioSource in sources)
            {
                if (audioSource.isActiveAndEnabled)
                    DestroyImmediate(audioSource);
            }

            sources.Clear();
        }

        private bool PlayInternal(AudioClip clip, bool loop)
        {
            var source = sources.FirstOrDefault(x => !x.enabled);
            if (source == default)
            {
                Debug.unityLogger.LogWarning(nameof(UnitySoundSource), $"Play limit reached, {clip.name}");
                return false;
            }

            source.enabled = true;
            source.loop = loop;
            source.clip = clip;
            source.Play();

            return true;
        }
        
        public void Play(AudioClip clip, bool loop)
        {
            if (clip == null)
                throw new ArgumentNullException();

            if (loop)
                PlayInternal(clip, true);
            else
                playQueue.Add(clip);
        }

        public void PlayCrossFade(AudioClip clip, bool loop)
        {
            if (clip == null)
                throw new ArgumentNullException();

            var stopSources = sources.Where(x => x.enabled);
            foreach (var stopSource in stopSources)
                stopSource.DOFade(0.0f, 0.2f)
                    .Play()
                    .OnComplete(() =>
                    {
                        stopSource.Stop();
                        stopSource.enabled = false;
                    });
            
            var playSource = sources.FirstOrDefault(x => !x.enabled);
            playSource.enabled = true;
            playSource.loop = loop;
            playSource.clip = clip;
            playSource.DOFade(1.0f, 0.2f).Play();
            playSource.Play();
        }

        public void Stop()
        {
            foreach (var source in sources)
            {
                source.Stop();
                source.enabled = false;
            }
        }

        public void Pause()
        {
            foreach (var source in sources.Where(x => x.enabled))
            {
                source.Pause();
            }
        }

        public void Resume()
        {
            foreach (var source in sources.Where(x => x.enabled))
            {
                source.UnPause();
            }
        }

        [PublicAPI]
        public sealed class Pool : MonoMemoryPool<int, AudioMixerGroup, UnitySoundSource>
        {
            protected override void Reinitialize(int playbackNumber, AudioMixerGroup group, UnitySoundSource item)
            {
                item.Initialize(playbackNumber, group);
            }

            protected override void OnDespawned(UnitySoundSource item)
            {
                item.Dispose();
            }
        }
    }
}