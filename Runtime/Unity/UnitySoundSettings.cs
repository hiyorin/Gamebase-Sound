using System;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;

namespace Gamebase.Sound.Unity
{
    [Serializable]
    public sealed class UnitySoundSettings
    {
        [Serializable]
        public sealed class Category
        {
            [SerializeField] private AudioMixerGroup audioMixerGroup = null;

            [SerializeField] private string volumeName = "";

            [SerializeField, Range(1, 10)] private int playbackNumber = 1;

            public AudioMixerGroup AudioMixerGroup => audioMixerGroup;

            public string VolumeName => volumeName;

            public int PlaybackNumber => playbackNumber;

            public Category(string volumeName, int playbackNumber)
            {
                this.volumeName = volumeName;
                this.playbackNumber = playbackNumber;
            }
        }
        
        [SerializeField] private AudioMixer audioMixer = null;

        [SerializeField, Range(-80.0f, 20.0f)] private float thresholdVolume = -40.0f;
        
        [SerializeField] private Category master = new Category("MasterVolume", 1);
        
        [SerializeField] private Category bgm = new Category("BgmVolume", 2);
        
        [SerializeField] private Category se = new Category("SeVolume", 3);
        
        [SerializeField] private Category voice = new Category("VoiceVolume", 3);
        
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
            builder.AppendLineFormat("  {0}: {1}", nameof(Master.PlaybackNumber), Master.PlaybackNumber);
            builder.AppendLineFormat("{0}", nameof(Bgm));
            builder.AppendLineFormat("  {0}: {1}", nameof(Bgm.AudioMixerGroup), Bgm.AudioMixerGroup != null ? bgm.AudioMixerGroup.name : "null");
            builder.AppendLineFormat("  {0}: {1}", nameof(Bgm.VolumeName), Bgm.VolumeName);
            builder.AppendLineFormat("  {0}: {1}", nameof(Bgm.PlaybackNumber), Bgm.PlaybackNumber);
            builder.AppendLineFormat("{0}", nameof(Se));
            builder.AppendLineFormat("  {0}: {1}", nameof(Se.AudioMixerGroup), Se.AudioMixerGroup != null ? Se.AudioMixerGroup.name : "null");
            builder.AppendLineFormat("  {0}: {1}", nameof(Se.VolumeName), Se.VolumeName);
            builder.AppendLineFormat("  {0}: {1}", nameof(Se.PlaybackNumber), Se.PlaybackNumber);
            builder.AppendLineFormat("{0}", nameof(Voice));
            builder.AppendLineFormat("  {0}: {1}", nameof(Voice.AudioMixerGroup), Voice.AudioMixerGroup != null ? Voice.AudioMixerGroup.name : "null");
            builder.AppendLineFormat("  {0}: {1}", nameof(Voice.VolumeName), Voice.VolumeName);
            builder.AppendLineFormat("  {0}: {1}", nameof(Voice.PlaybackNumber), Voice.PlaybackNumber);
            return builder.ToString();
        }
    }
}