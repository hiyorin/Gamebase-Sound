#if GAMEBASE_ADD_ADX2
using JetBrains.Annotations;

namespace Gamebase.Sound.Adx2.Internal
{
    [PublicAPI]
    internal sealed class Adx2SoundVolumeController : ISoundVolumeController
    {
        private readonly Adx2CategoryVolumeSettings settings;

        public Adx2SoundVolumeController(Adx2CategoryVolumeSettings settings)
        {
            this.settings = settings;
        }
        
        #region ISoundVolumeController implementation
        
        public float MasterVolume
        {
            get => CriAtom.GetCategoryVolume(settings.MasterName);
            set => CriAtom.SetCategoryVolume(settings.MasterName, value); 
        }

        public float BgmVolume
        {
            get => CriAtom.GetCategoryVolume(settings.BgmName);
            set => CriAtom.SetCategoryVolume(settings.BgmName, value);
        }

        public float SeVolume
        {
            get => CriAtom.GetCategoryVolume(settings.SeName);
            set => CriAtom.SetCategoryVolume(settings.SeName, value);
        }

        public float VoiceVolume
        {
            get => CriAtom.GetCategoryVolume(settings.VoiceName);
            set => CriAtom.SetCategoryVolume(settings.VoiceName, value);
        }
        
        public void Set(SoundVolume value)
        {
            MasterVolume = value.Master;
            BgmVolume = value.Bgm;
            SeVolume = value.Bgm;
            VoiceVolume = value.Voice;
        }
        
        #endregion
    }
}
#endif