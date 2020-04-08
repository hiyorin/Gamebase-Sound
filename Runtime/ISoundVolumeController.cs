namespace Gamebase.Sound
{
    public interface ISoundVolumeController
    {
        float MasterVolume { set; get; }
        
        float BgmVolume { set; get; }
        
        float SeVolume { set; get; }
        
        float VoiceVolume { set; get; }
        
        void Set(SoundVolume value);
    }
}