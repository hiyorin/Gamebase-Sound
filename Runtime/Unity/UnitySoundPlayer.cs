using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Gamebase.Sound.Unity
{
    [PublicAPI]
    public sealed class UnitySoundPlayer : ISoundPlayer
    {
        private UnitySoundPack soundPack;
        
        private UnitySoundSource bgmSource, seSource, voiceSource;

        private void Initialize(UnitySoundPack pack, UnitySoundSource bgm, UnitySoundSource se, UnitySoundSource voice)
        {
            if (pack == null || bgm == null || se == null || voice == null)
                throw new ArgumentNullException();
            
            Dispose();

            soundPack = pack;
            bgmSource = bgm;
            seSource = se;
            voiceSource = voice;
        }
        
        private void Dispose()
        {
            soundPack = null;
            bgmSource = null;
            seSource = null;
            voiceSource = null;
        }
        
        #region ISoundPlayer implementation

        string ISoundPlayer.FileName => soundPack.name;
        
        void ISoundPlayer.Play(string soundName, bool loop)
        {
            AudioClip clip = null;
            if ((clip = soundPack.BgmClips.FirstOrDefault(x => x.name == soundName)) != null)
                bgmSource.PlayCrossFade(clip, loop);
            else if ((clip = soundPack.SeClips.FirstOrDefault(x => x.name == soundName)) != null)
                seSource.Play(clip, loop);
            else if ((clip = soundPack.VoiceClips.FirstOrDefault(x => x.name == soundName)) != null)
                voiceSource.Play(clip, loop);
            else
                throw new KeyNotFoundException();
        }

        void ISoundPlayer.Pause()
        {
            bgmSource.Pause();
            seSource.Pause();
            voiceSource.Pause();
        }

        void ISoundPlayer.Resume()
        {
            bgmSource.Resume();
            seSource.Resume();
            voiceSource.Resume();
        }

        void ISoundPlayer.Stop()
        {
            bgmSource.Stop();
            seSource.Stop();
            voiceSource.Stop();
        }
        
        #endregion

        public sealed class Pool : MemoryPool<UnitySoundPack, UnitySoundSource, UnitySoundSource, UnitySoundSource, UnitySoundPlayer>
        {
            protected override void Reinitialize(UnitySoundPack pack, UnitySoundSource bgm, UnitySoundSource se, UnitySoundSource voice, UnitySoundPlayer item)
            {
                item.Initialize(pack, bgm, se, voice);
            }

            protected override void OnDespawned(UnitySoundPlayer item)
            {
                item.Dispose();
            }
        }
    }
}