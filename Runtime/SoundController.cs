#if false
using System.Collections;
using UnityEngine;
using Zenject;

namespace Gamebase.Sound
{
    public sealed class SoundController : MonoBehaviour
    {
        [Inject] private ISoundManager soundManager = null;
        
        private ISoundPlayer player;
        
        #region ActionTask implementation

        public IEnumerator Load(string fileName)
        {
            if (player != null)
            {
                soundManager.Unload(player);
                player = null;
            }

            yield return soundManager.Load(fileName).WaitForCompletion(x => player = x);
        }
        
        public void Play(string soundName, bool loop)
        {
            player?.Play(soundName, loop);
        }

        public void Stop()
        {
            player?.Stop();
        }

        public void Pause()
        {
            player?.Pause();
        }

        public void Resume()
        {
            player?.Resume();
        }
        
        #endregion
    }
}
#endif