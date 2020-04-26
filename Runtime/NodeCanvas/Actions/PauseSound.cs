#if GAMEBASE_ADD_NODECANVAS
using JetBrains.Annotations;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Gamebase.Sound.NodeCanvas.Actions
{
    [PublicAPI]
    [Name("Pause Sound")]
    [Category("✫ Gamebase/Sound")]
    public sealed class PauseSound : ActionTask
    {
        [BlackboardOnly] public BBParameter<ISoundPlayer> soundPlayer = default;
        
        protected override void OnExecute()
        {
            soundPlayer.value.Pause();
            EndAction(true);
        }
    }
}
#endif