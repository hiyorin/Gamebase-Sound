using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace Gamebase.Sound.Unity
{
    public sealed class UnitySoundSource : MonoBehaviour
    {
        private readonly List<AudioSource> sources = new List<AudioSource>();

        public int PlaybackNumber => sources.Count;

        private void Update()
        {
            foreach (var source in sources)
            {
                if (!source.enabled)
                    continue;

                if (source.time >= source.clip.length)
                    source.enabled = false;
            }
        }

        public void Initialize(int playbackNumber, AudioMixerGroup group)
        {
            for (var i = 0; i < playbackNumber; i++)
            {
                var source = gameObject.AddComponent<AudioSource>();
                source.enabled = false;
                source.outputAudioMixerGroup = group;
                
                sources.Add(source);
            }
        }

        public void Play(AudioClip clip, bool loop)
        {
            if (clip == null)
                throw new ArgumentNullException();
            
            var source = sources.FirstOrDefault(x => !x.enabled);
            if (source == null)
            {
                Debug.unityLogger.LogWarning(nameof(UnitySoundSource), $"Play limit reached, {clip.name}");
                return;
            }

            source.enabled = true;
            source.loop = loop;
            source.clip = clip;
            source.Play();
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
    }
}