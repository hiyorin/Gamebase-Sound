using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamebase.Sound.Unity
{
    [CreateAssetMenu(fileName = "UnitySoundPack", menuName = "Gamebase-Sound/UnitySoundPack")]
    public sealed class UnitySoundPack : ScriptableObject
    {
        [SerializeField] private AudioClip[] bgmClips = { };

        [SerializeField] private AudioClip[] seClips = { };

        [SerializeField] private AudioClip[] voiceClips = { };

        public IEnumerable<AudioClip> BgmClips => bgmClips;

        public IEnumerable<AudioClip> SeClips => seClips;

        public IEnumerable<AudioClip> VoiceClips => voiceClips;
    }
}