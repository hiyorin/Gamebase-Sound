using UnityEngine;
using Zenject;

namespace Gamebase.Sound.NodeCanvas
{
    public sealed class SoundTaskManager : MonoBehaviour
    {
        [Inject] internal ISoundManager Manager { get; } = default;

        [Inject] internal ISoundVolumeController VolumeController { get; } = default;
    }
}