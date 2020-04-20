#if GAMEBASE_ADD_MASTERAUDIO
using UnityEngine;
using Zenject;

namespace Gamebase.Sound.MasterAudio
{
    internal sealed class MasterAudioVolumeController : ISoundVolumeController
    {
        private const float MaxDb = 0.0f;
        
        [Inject] private MasterAudioSettings settings = null;

        private float GetVolume(string name)
        {
            if (!settings.AudioMixer.GetFloat(name, out var volume))
                Debug.unityLogger.LogError(nameof(MasterAudioVolumeController), $"{nameof(GetVolume)} failed. {name} is not found. {volume}");
            return Mathf.InverseLerp(settings.ThresholdVolume, MaxDb, volume);
        }

        private void SetVolume(string name, float value)
        {
            var volume = Mathf.Lerp(settings.ThresholdVolume, MaxDb, Mathf.Clamp01(value));
            if (!settings.AudioMixer.SetFloat(name, volume))
                Debug.unityLogger.LogError(nameof(MasterAudioVolumeController), $"{nameof(SetVolume)} failed. {name} is not found. {volume}");
        }
        
        #region ISoundVolumeController implementation
        
        public float MasterVolume
        {
            get => GetVolume(settings.Master.VolumeName);
            set => SetVolume(settings.Master.VolumeName, value);
        }

        public float BgmVolume
        {
            get => GetVolume(settings.Bgm.VolumeName);
            set => SetVolume(settings.Bgm.VolumeName, value);
        }

        public float SeVolume
        {
            get => GetVolume(settings.Se.VolumeName);
            set => SetVolume(settings.Se.VolumeName, value);
        }

        public float VoiceVolume
        {
            get => GetVolume(settings.Voice.VolumeName);
            set => SetVolume(settings.Voice.VolumeName, value);
        }
        
        public void Set(SoundVolume value)
        {
            MasterVolume = value.Master;
            BgmVolume = value.Bgm;
            SeVolume = value.Se;
            VoiceVolume = value.Voice;
        }
        
        #endregion
    }
}
#endif