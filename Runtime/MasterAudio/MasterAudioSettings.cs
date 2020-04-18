#if GAMEBASE_ADD_MASTERAUDIO
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;

namespace Gamebase.Sound.MasterAudio
{
    [Serializable]
    public sealed class MasterAudioSettings
    {
        [Serializable]
        public sealed class Category
        {
            [SerializeField] private AudioMixerGroup audioMixerGroup = default;

            [SerializeField] private string volumeName = default;

            public AudioMixerGroup AudioMixerGroup => audioMixerGroup;

            public string VolumeName => volumeName;

            public Category(string volumeName)
            {
                this.volumeName = volumeName;
            }
        }
        
        [SerializeField] private AudioMixer audioMixer = default;

        [SerializeField, Range(-80.0f, 20.0f)] private float thresholdVolume = -40.0f;
        
        [SerializeField] private Category master = new Category("MasterVolume");
        
        [SerializeField] private Category bgm = new Category("BgmVolume");
        
        [SerializeField] private Category se = new Category("SeVolume");
        
        [SerializeField] private Category voice = new Category("VoiceVolume");
        
        public AudioMixer AudioMixer => audioMixer;

        public float ThresholdVolume => thresholdVolume;

        public Category Master => master;
        
        public Category Bgm => bgm;

        public Category Se => se;

        public Category Voice => voice;

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLineFormat("{0}: {1}", nameof(AudioMixer), AudioMixer != null ? AudioMixer.name : "null");
            builder.AppendLineFormat("{0}: {1}", nameof(ThresholdVolume), ThresholdVolume);
            builder.AppendLineFormat("{0}", nameof(Master));
            builder.AppendLineFormat("  {0}: {1}", nameof(Master.AudioMixerGroup), Master.AudioMixerGroup != null ? Master.AudioMixerGroup.name : "null");
            builder.AppendLineFormat("  {0}: {1}", nameof(Master.VolumeName), Master.VolumeName);
            builder.AppendLineFormat("{0}", nameof(Bgm));
            builder.AppendLineFormat("  {0}: {1}", nameof(Bgm.AudioMixerGroup), Bgm.AudioMixerGroup != null ? bgm.AudioMixerGroup.name : "null");
            builder.AppendLineFormat("  {0}: {1}", nameof(Bgm.VolumeName), Bgm.VolumeName);
            builder.AppendLineFormat("{0}", nameof(Se));
            builder.AppendLineFormat("  {0}: {1}", nameof(Se.AudioMixerGroup), Se.AudioMixerGroup != null ? Se.AudioMixerGroup.name : "null");
            builder.AppendLineFormat("  {0}: {1}", nameof(Se.VolumeName), Se.VolumeName);
            builder.AppendLineFormat("{0}", nameof(Voice));
            builder.AppendLineFormat("  {0}: {1}", nameof(Voice.AudioMixerGroup), Voice.AudioMixerGroup != null ? Voice.AudioMixerGroup.name : "null");
            builder.AppendLineFormat("  {0}: {1}", nameof(Voice.VolumeName), Voice.VolumeName);
            return builder.ToString();
        }
    }
}
#endif