#if GAMEBASE_ADD_MASTERAUDIO
using System.Collections.Generic;
using System.Linq;
using DarkTonic.MasterAudio;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;
using MA = DarkTonic.MasterAudio.MasterAudio;

namespace Gamebase.Sound.MasterAudio.Internal
{
    [RequireComponent(typeof(PlaylistController))]
    internal class MasterAudioPlayer : MonoBehaviour, ISoundPlayer
    {
        private string fileName;

        private DynamicSoundGroupCreator sound;
        
        private PlaylistController controller;
        
        public List<string> playlistNames = new List<string>();
        
        public List<string> groupMixerNames = new List<string>();
        
        private void Awake()
        {
            controller = GetComponent<PlaylistController>();
        }
        
        private void Initialize(string fileName, DynamicSoundGroupCreator sound, AudioMixerGroup mixerChannel)
        {
            this.fileName = fileName;
            this.sound = Instantiate(sound, transform);

            controller.RouteToMixerChannel(mixerChannel);
            
            playlistNames.AddRange(sound.musicPlaylists.Select(x => x.playlistName));
            groupMixerNames.AddRange(sound.GroupsToCreate.Select(x => x.name));
        }

        private void Dispose()
        {
            Destroy(sound);
            sound = null;
        }
        
        #region ISoundPlayer implementaion
 
        string ISoundPlayer.FileName => fileName;

        void ISoundPlayer.Play(string soundName, bool loop)
        {
            if (playlistNames.Contains(soundName))
            {
                controller.loopPlaylist = loop;
                controller.ChangePlaylist(soundName);
            }
            else
            {
                MA.PlaySoundAndForget(soundName);
            }
        }

        void ISoundPlayer.Pause()
        {
            controller.PausePlaylist();
        }
        
        void ISoundPlayer.Resume()
        {
            controller.UnpausePlaylist();
        }

        void ISoundPlayer.Stop()
        {
            controller.StopPlaylist();
        }
        
        #endregion

        [PublicAPI]
        public sealed class Pool : MonoMemoryPool<string, DynamicSoundGroupCreator, AudioMixerGroup, MasterAudioPlayer>
        {
            protected override void Reinitialize(string fileName, DynamicSoundGroupCreator sound, AudioMixerGroup mixerChannel, MasterAudioPlayer item)
            {
                item.Initialize(fileName, sound, mixerChannel);
            }

            protected override void OnDespawned(MasterAudioPlayer item)
            {
                item.Dispose();
            }
        }
    }
}
#endif