#if GAMEBASE_ADD_ADX2
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Gamebase.Sound.Adx2
{
    [RequireComponent(typeof(CriAtomSource))]
    public sealed class Adx2SoundPlayer : MonoBehaviour, ISoundPlayer
    {   
        private CriAtomSource source;

        private void Awake()
        {
            source = GetComponent<CriAtomSource>();
        }
        
        private void OnDestroy()
        {
            Dispose();
        }
        
        private void Initialize(string cueSheet)
        {
            Dispose();
            source.cueSheet = cueSheet;
        }

        private void Dispose()
        {
            source.cueSheet = null;
        }
        
        #region ISoundPlayer implemeantaion

        string ISoundPlayer.FileName => source != null ? source.cueName : null;
        
        void ISoundPlayer.Play(string soundName, bool loop)
        {
            source.Pause(false);
            source.loop = loop;
            source.Play(soundName);
        }

        void ISoundPlayer.Pause()
        {
            source.Pause(true);
        }

        void ISoundPlayer.Resume()
        {
            source.Pause(false);
        }
        
        void ISoundPlayer.Stop()
        {
            source.Stop();
        }
        
        #endregion

        [PublicAPI]
        public sealed class Pool : MonoMemoryPool<string, Adx2SoundPlayer>
        {
            protected override void Reinitialize(string cueSheet, Adx2SoundPlayer item)
            {
                item.Initialize(cueSheet);
            }

            protected override void OnDespawned(Adx2SoundPlayer item)
            {
                item.Dispose();
            }
        }
    }
}
#endif