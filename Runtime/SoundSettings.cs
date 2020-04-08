using System;
using UnityEngine;

namespace Gamebase.Sound
{
    [Serializable]
    public sealed class SoundSettings
    {
        [SerializeField] private SoundVolume defaultVolume = new SoundVolume(1.0f, 1.0f, 1.0f, 1.0f);

        public SoundVolume DefaultVolume => defaultVolume;

        public override string ToString()
        {
            return $"{nameof(DefaultVolume)}: {DefaultVolume}";
        }
    }

    [Serializable]
    public struct SoundVolume
    {
        [Range(0.0f, 1.0f)] public float Master;

        [Range(0.0f, 1.0f)] public float Bgm;

        [Range(0.0f, 1.0f)] public float Se;

        [Range(0.0f, 1.0f)] public float Voice;

        public SoundVolume(float master, float bgm, float se, float voice)
        {
            Master = master;
            Bgm = bgm;
            Se = se;
            Voice = voice;
        }

        public override string ToString()
        {
            return $"{nameof(SoundVolume)} Master:{Master}, BGM:{Bgm}, SE:{Se}, Voice:{Voice}";
        }
    }
}