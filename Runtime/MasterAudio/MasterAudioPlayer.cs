#if GAMEBASE_ADD_MASTERAUDIO
using System;
using System.Collections.Generic;
using System.Linq;
using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.Audio;
using Sound = DarkTonic.MasterAudio.MasterAudio;

namespace Gamebase.Sound.MasterAudio
{
    [RequireComponent(typeof(PlaylistController))]
    internal class MasterAudioPlayer : MonoBehaviour, ISoundPlayer, IDisposable
    {
        private string fileName;

        private bool isInitialized = false;

        private DynamicSoundGroupCreator owner;
        
        private PlaylistController controller;
        
        private readonly List<string> playlistNames = new List<string>();
        
        private readonly List<string> groupMixerNames = new List<string>();
        
        private void Awake()
        {
            controller = GetComponent<PlaylistController>();
        }
        
        public void Initialize(string fileName, DynamicSoundGroupCreator owner, AudioMixerGroup mixerChannel)
        {
            this.fileName = fileName;
            this.owner = owner;

            controller.RouteToMixerChannel(mixerChannel);
            
            playlistNames.AddRange(owner.musicPlaylists.Select(x => x.playlistName));
            groupMixerNames.AddRange(owner.GroupsToCreate.Select(x => x.name));
            
            isInitialized = true;
        }

        public void Dispose()
        {
            isInitialized = false;
        }
        
        #region ISoundPlayer implementaion

        public bool IsValid => isInitialized;
        
        public string FileName => fileName;

        public IEnumerable<string> SoundNames => playlistNames.Union(groupMixerNames);

        public void Play(string soundName, bool loop)
        {
            if (playlistNames.Contains(soundName))
            {
                controller.loopPlaylist = loop;
                controller.ChangePlaylist(soundName);
            }
            else
            {
                Sound.PlaySoundAndForget(soundName);
            }
        }

        public void Pause()
        {
            controller.PausePlaylist();
        }

        public void Resume()
        {
            controller.UnpausePlaylist();
        }

        public void Stop()
        {
            controller.StopPlaylist();
        }
        
        #endregion
    }
}
#endif