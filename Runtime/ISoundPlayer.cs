namespace Gamebase.Sound
{
    public interface ISoundPlayer
    {
        /// <summary>
        /// 再生
        /// </summary>
        /// <param name="soundName"></param>
        /// <param name="loop"></param>
        void Play(string soundName, bool loop);

        /// <summary>
        /// 一時停止
        /// </summary>
        void Pause();

        /// <summary>
        /// 再開
        /// </summary>
        void Resume();
        
        /// <summary>
        /// 停止
        /// </summary>
        void Stop();
    }
}