#if GAMEBASE_ADD_NODECANVAS
using JetBrains.Annotations;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UniRx.Async;

namespace Gamebase.Sound.NodeCanvas.Actions
{
    [PublicAPI]
    [Name("Load Sound")]
    [Category("✫ Gamebase/Sound")]
    public sealed class LoadSound : ActionTask<SoundTaskManager>
    {
        [RequiredField] public BBParameter<string> path = default;

        [BlackboardOnly] public BBParameter<ISoundPlayer> saveAs = default;
        
        protected override void OnExecute()
        {
            Load().Forget();
        }

        private async UniTask Load()
        {
            saveAs.value = await agent.Manager.Load(path.value);
            EndAction(true);
        }
    }
}
#endif