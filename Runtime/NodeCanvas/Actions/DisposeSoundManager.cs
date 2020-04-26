#if GAMEBASE_ADD_NODECANVAS
using JetBrains.Annotations;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Gamebase.Sound.NodeCanvas.Actions
{
    [PublicAPI]
    [Name("Dispose Sound Manager")]
    [Category("✫ Gamebase/Sound")]
    public sealed class DisposeSoundManager : ActionTask<SoundTaskManager>
    {
        protected override void OnExecute()
        {
            agent.Manager.Dispose();
        }
    }
}
#endif