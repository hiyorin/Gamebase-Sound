using System.Collections.Generic;
using UniRx.Async;

namespace Gamebase.Sound
{
    public interface ISoundManager
    {
        /// <summary>
        /// <para>サウンドファイルをロード</para>
        /// <para>Playerを生成して返す</para>
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        UniTask<ISoundPlayer> Load(string fileName);

        /// <summary>
        /// <para>サウンドファイルのアンロード</para>
        /// <para>生成したPlayerの返却</para>
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        bool Unload(ISoundPlayer player);
        
        /// <summary>
        /// サウンドファイルのみのロード
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        UniTask<IList<string>> PreLoad(string fileName);
        
        /// <summary>
        /// サウンドファイルのみのアンロード
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool PreUnload(string fileName);
        
        /// <summary>
        /// サウンドファイルのキャッシュ状況を取得
        /// </summary>
        /// <param name="dst"></param>
        void GetCacheInfos(ref IList<string> dst);
    }
}