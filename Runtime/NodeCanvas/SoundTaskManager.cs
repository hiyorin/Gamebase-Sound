#if GAMEBASE_ADD_NODECANVAS
using UnityEngine;
using Zenject;

namespace Gamebase.Sound.NodeCanvas
{
    public sealed class SoundTaskManager : MonoBehaviour
    {
        [Inject] public ISoundManager Manager { get; } = default;

        [Inject] internal ISoundVolumeController VolumeController { get; } = default;
    }
}
#endif