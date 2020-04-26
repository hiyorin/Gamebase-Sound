#if GAMEBASE_ADD_MASTERAUDIO
using Gamebase.Sound.MasterAudio.Internal;
using UnityEngine;
using Zenject;
using MA = DarkTonic.MasterAudio.MasterAudio;

namespace Gamebase.Sound.MasterAudio
{
    [CreateAssetMenu(fileName = "MasterAudioInstaller", menuName = "Installers/Gamebase/MasterAudioInstaller")]
    public sealed class MasterAudioInstaller : ScriptableObjectInstaller<MasterAudioInstaller>
    {
        [SerializeField] private MA masterAudio = default;
        
        [SerializeField] private SoundSettings generalSettings = default;
        
        [SerializeField] private MasterAudioSettings settings = default;
        
        public override void InstallBindings()
        {
            var subContainer = Container.CreateSubContainer();
            InstallSubContainer(subContainer);

            Container.BindInterfacesTo<MasterAudioManager>()
                .FromSubContainerResolve()
                .ByInstance(subContainer)
                .AsSingle();
            
            Container.BindInterfacesTo<MasterAudioVolumeController>()
                .FromSubContainerResolve()
                .ByInstance(subContainer)
                .AsSingle();
        }

        private void InstallSubContainer(DiContainer subContainer)
        {
            subContainer.Bind<MasterAudioManager>().AsSingle();
            subContainer.Bind<MasterAudioVolumeController>().AsSingle();
            
            Container.BindInstance(masterAudio).AsSingle();
            Container.BindInstance(generalSettings).AsSingle();
            Container.BindInstance(settings).AsSingle();
            
            // Factories
            Container.BindMemoryPool<MasterAudioPlayer, MasterAudioPlayer.Pool>()
                .FromNewComponentOnNewPrefabResource(nameof(MasterAudioPlayer));
        }
    }
}
#endif