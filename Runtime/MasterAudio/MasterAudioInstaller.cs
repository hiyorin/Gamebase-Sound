#if GAMEBASE_ADD_MASTERAUDIO
using UnityEngine;
using Zenject;
using Sound = DarkTonic.MasterAudio.MasterAudio;

namespace Gamebase.Sound.MasterAudio
{
    [CreateAssetMenu(fileName = "MasterAudioInstaller", menuName = "Installers/Gamebase-Extensions/MasterAudioInstaller")]
    public sealed class MasterAudioInstaller : ScriptableObjectInstaller<MasterAudioInstaller>
    {
        [SerializeField] private Sound masterAudio = default;
        
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
        }
    }
}
#endif