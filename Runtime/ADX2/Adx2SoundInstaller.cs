#if GAMEBASE_ADD_ADX2
using System;
using UnityEngine;
using Zenject;

namespace Gamebase.Sound.Adx2
{
    [CreateAssetMenu(fileName = "Adx2SoundInstaller", menuName = "Installers/Adx2SoundInstaller")]
    public sealed class Adx2SoundInstaller : ScriptableObjectInstaller<Adx2SoundInstaller>
    {
        [SerializeField] private SoundSettings generalSettings = null;
        
        [SerializeField] private Adx2Settings adx2Settings = null;

        [SerializeField] private Adx2CategoryVolumeSettings categoryVolumeSettings = null;

        [SerializeField] private CriWareInitializer criInitializer = null;
        
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<Adx2SoundManager>().AsSingle();
            Container.BindInterfacesTo<Adx2SoundVolumeController>().AsSingle();
            // Container.BindInterfacesTo<SoundConfigRepository>().AsSingle();
            // Container.BindInterfacesTo<SoundConfigUseCase>().AsSingle();
            Container.BindInstance(generalSettings).AsSingle();
            Container.BindInstance(adx2Settings).AsSingle();
            Container.BindInstance(categoryVolumeSettings).AsSingle();
            Container.BindInstance(criInitializer).AsSingle();
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