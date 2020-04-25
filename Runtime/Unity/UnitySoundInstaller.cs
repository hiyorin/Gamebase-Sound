using Gamebase.Sound.Unity.Internal;
using UnityEngine;
using Zenject;

namespace Gamebase.Sound.Unity
{
    [CreateAssetMenu(fileName = "UnitySoundInstaller", menuName = "Installers/Gamebase/UnitySoundInstaller")]
    public sealed class UnitySoundInstaller : ScriptableObjectInstaller<UnitySoundInstaller>
    {
        [SerializeField] private SoundSettings generalSettings = default;

        [SerializeField] private UnitySoundSettings settings = default;

        public override void InstallBindings()
        {
            var subContainer = Container.CreateSubContainer();
            InstallSubContainer(subContainer);

            Container.BindInterfacesTo<UnitySoundManager>()
                .FromSubContainerResolve()
                .ByInstance(subContainer)
                .AsSingle();

            Container.BindInterfacesTo<UnitySoundVolumeController>()
                .FromSubContainerResolve()
                .ByInstance(subContainer)
                .AsSingle();
        }

        private void InstallSubContainer(DiContainer subContainer)
        {
            // Managers
            subContainer.Bind<UnitySoundManager>().AsSingle();
            subContainer.Bind<UnitySoundVolumeController>().AsSingle();

            // Settings
            subContainer.BindInstance(generalSettings).AsSingle();
            subContainer.BindInstance(settings).AsSingle();
            
            // Factories
            subContainer.BindMemoryPool<UnitySoundSource, UnitySoundSource.Pool>()
                .FromNewComponentOnNewPrefabResource(nameof(UnitySoundSource));
            subContainer.BindMemoryPool<UnitySoundPlayer, UnitySoundPlayer.Pool>();
        }
    }
}
