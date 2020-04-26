#if GAMEBASE_ADD_ADX2
using System;
using Gamebase.Sound.Adx2.Internal;
using UnityEngine;
using Zenject;

namespace Gamebase.Sound.Adx2
{
    [CreateAssetMenu(fileName = "Adx2SoundInstaller", menuName = "Installers/Gamebase/Adx2SoundInstaller")]
    public sealed class Adx2SoundInstaller : ScriptableObjectInstaller<Adx2SoundInstaller>
    {
        [SerializeField] private SoundSettings generalSettings = null;
        
        [SerializeField] private Adx2Settings adx2Settings = null;

        [SerializeField] private Adx2CategoryVolumeSettings categoryVolumeSettings = null;

        [SerializeField] private CriWareInitializer criInitializer = null;
        
        public override void InstallBindings()
        {
            var subContainer = Container.CreateSubContainer();
            InstallSubContainer(subContainer);
            
            Container.BindInterfacesTo<Adx2SoundManager>()
                .FromSubContainerResolve()
                .ByInstance(subContainer)
                .AsSingle();
            
            Container.BindInterfacesTo<Adx2SoundVolumeController>()
                .FromSubContainerResolve()
                .ByInstance(subContainer)
                .AsSingle();
        }

        private void InstallSubContainer(DiContainer subContainer)
        {
            // Managers
            subContainer.Bind<Adx2SoundManager>().AsSingle();
            subContainer.Bind<Adx2SoundVolumeController>().AsSingle();
            
            // Settings
            subContainer.BindInstance(generalSettings).AsSingle();
            subContainer.BindInstance(adx2Settings).AsSingle();
            subContainer.BindInstance(categoryVolumeSettings).AsSingle();
            subContainer.Bind<CriAtom>().FromResources("CRIAtom").AsSingle();
            subContainer.BindInstance(criInitializer).AsSingle();
            
            // Factories
            subContainer.BindMemoryPool<Adx2SoundPlayer, Adx2SoundPlayer.Pool>()
                .FromNewComponentOnNewPrefabResource(nameof(Adx2SoundPlayer));
        }
    }

    [Serializable]
    public sealed class Adx2Settings
    {
        [SerializeField] private AcfAssetReference defaultAcfReference = null;

        [SerializeField] private CueSheetReference preloadCueSheetReference = null;
        
        public AcfAssetReference DefaultAcfReference => defaultAcfReference;

        public CueSheetReference PreloadCueSheetReference => preloadCueSheetReference;
    }

    [Serializable]
    public sealed class Adx2CategoryVolumeSettings
    {
        [SerializeField] private string masterName = "Master";

        [SerializeField] private string bgmName = "BGM";

        [SerializeField] private string seName = "SE";
        
        [SerializeField] private string voiceName = "Voice";

        public string MasterName => masterName;

        public string BgmName => bgmName;

        public string SeName => seName;

        public string VoiceName => voiceName;
    }

    [Serializable]
    public sealed class CueSheetReference
    {
        [SerializeField] private string cueSheetName = null;
        
        [SerializeField] private AcbAssetReference acbReference = null;

        [SerializeField] private AwbAssetReference awbReference = null;

        public string CueSheetName => cueSheetName;

        public AcbAssetReference AcbReference => acbReference;

        public AwbAssetReference AwbReference => awbReference;
    }
}
#endif