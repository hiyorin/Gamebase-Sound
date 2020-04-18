#if GAMEBASE_ADD_ADX2
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamebase.Sound.Adx2
{
    public sealed class Adx2SoundPlayer : MonoBehaviour, ISoundPlayer, IDisposable
    {   
        private CriAtomSource source;

        private IEnumerable<string> soundNames;

        private bool isInitialize = false;
        
        private void OnDestroy()
        {
            Dispose();
        }
        
        public void Initialize(string cueSheet, IEnumerable<string> cueNames)
        {
            Dispose();
            
            source = gameObject.AddComponent<CriAtomSource>();
            source.cueSheet = cueSheet;
            soundNames = cueNames;

            isInitialize = true;
        }

        public void Dispose()
        {
            if (source != null)
            {
                Destroy(source);
                source = null;
            }

            soundNames = null;
            isInitialize = false;
        }
        
        #region ISoundPlayer implemeantaion

        public bool IsValid => isInitialize;
        
        public string FileName => source != null ? source.cueName : null;

        public IEnumerable<string> SoundNames => soundNames;

        public void Play(string soundName, bool loop)
        {
            if (source == null)
                return;
         
            source.Pause(false);
            source.loop = loop;
            source.Play(soundName);
        }

        public void Pause()
        {
            if (source == null)
                return;
            
            source.Pause(true);
        }

        public void Resume()
        {
            if (source == null)
                return;
            
            source.Pause(false);
        }
        
        public void Stop()
        {
            if (source == null)
                return;
            
            source.Stop();
        }
        
        #endregion
    }
}
#endif