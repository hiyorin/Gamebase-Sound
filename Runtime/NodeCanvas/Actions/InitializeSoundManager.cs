#if GAMEBASE_ADD_NODECANVAS
using JetBrains.Annotations;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Gamebase.Sound.NodeCanvas.Actions
{
    [PublicAPI]
    [Name("Initialize Sound Manager")]
    [Category("✫ Gamebase/Sound")]
    public sealed class InitializeSoundManager : ActionTask<SoundTaskManager>
    {
        protected override void OnExecute()
        {
            agent.Manager.Initialize().GetAwaiter().OnCompleted(() => EndAction(true));
        }
    }
}
#endif