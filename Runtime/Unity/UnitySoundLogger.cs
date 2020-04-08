using UnityEngine;
using Zenject;

namespace Gamebase.Sound.Unity
{
    public sealed class UnitySoundLogger : IInitializable
    {
        [Inject] private SoundSettings generalSettings = null;
        
        [Inject] private UnitySoundSettings settings = null;
        
        void IInitializable.Initialize()
        {
            var tag = GetType().Name;
            Debug.unityLogger.Log(tag, $"{nameof(generalSettings)}\n{generalSettings}");
            Debug.unityLogger.Log(tag, $"{nameof(settings)}\n{settings}");
        }
    }
}