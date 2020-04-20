using UniRx.Async;

namespace Gamebase.Sound
{
    public interface ISoundManager
    {
        UniTask Initialize();

        void Dispose();

        UniTask<ISoundPlayer> Load(string path);

        void Unload(ISoundPlayer player);
    }
}