using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamebase.Sound.Unity
{
    public sealed class UnitySoundPlayer : MonoBehaviour, ISoundPlayer, IDisposable
    {
        private string fileNameCache;

        private UnitySoundSource bgmSourceCache, seSourceCache, voiceSourceCache;
        
        private UnitySoundPack packCache;

        private IEnumerable<string> soundNamesCache;

        private bool initialized = false;
        
        private void OnDestroy()
        {
            Dispose();
        }

        public void Initialize(string fileName, UnitySoundPack pack, IEnumerable<string> soundNames, UnitySoundSource bgmSource, UnitySoundSource seSource, UnitySoundSource voiceSource)
        {
            if (pack == null || soundNames == null || bgmSource == null || seSource == null || voiceSource == null)
                throw new ArgumentNullException();
            
            Dispose();

            fileNameCache = fileName;
            bgmSourceCache = bgmSource;
            seSourceCache = seSource;
            voiceSourceCache = voiceSource;
            packCache = pack;
            soundNamesCache = soundNames;

            initialized = true;
        }
        
        public void Dispose()
        {
            fileNameCache = null;
            bgmSourceCache = null;
            seSourceCache = null;
            voiceSourceCache = null;
            packCache = null;
            soundNamesCache = null;

            initialized = false;
        }
        
        #region ISoundPlayer implementation

        public bool IsValid => initialized;
        
        public string FileName => fileNameCache;

        public IEnumerable<string> SoundNames => soundNamesCache;

        public void Play(string soundName, bool loop)
        {
            if (!initialized)
                throw new Exception("Not initialized");

            AudioClip clip = null;
            if ((clip = packCache.BgmClips.FirstOrDefault(x => x.name == soundName)) != null)
                bgmSourceCache.PlayCrossFade(clip, loop);
            else if ((clip = packCache.SeClips.FirstOrDefault(x => x.name == soundName)) != null)
                seSourceCache.Play(clip, loop);
            else if ((clip = packCache.VoiceClips.FirstOrDefault(x => x.name == soundName)) != null)
                voiceSourceCache.Play(clip, loop);
            else
                throw new KeyNotFoundException();
        }

        public void Pause()
        {
            if (!initialized)
                throw new Exception("Not initialized");
            
            bgmSourceCache.Pause();
            seSourceCache.Pause();
            voiceSourceCache.Pause();
        }

        public void Resume()
        {
            if (!initialized)
                throw new Exception("Not initialized");
            
            bgmSourceCache.Resume();
            seSourceCache.Resume();
            voiceSourceCache.Resume();
        }

        public void Stop()
        {
            if (!initialized)
                throw new Exception("Not initialized");
         
            bgmSourceCache.Stop();
            seSourceCache.Stop();
            voiceSourceCache.Stop();
        }
        
        #endregion
    }
}