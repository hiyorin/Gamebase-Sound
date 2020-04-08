using System.Collections.Generic;

namespace Gamebase.Sound
{
    public interface ISoundPlayer
    {
        /// <summary>
        /// 使用可能フラグ
        /// </summary>
        bool IsValid { get; }
        
        /// <summary>
        /// サウンドファイル名
        /// </summary>
        string FileName { get; }
        
        /// <summary>
        /// 収録されているサウンド一覧
        /// </summary>
        IEnumerable<string> SoundNames { get; }

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