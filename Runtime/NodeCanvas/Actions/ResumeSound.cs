#if GAMEBASE_ADD_NODECANVAS
using JetBrains.Annotations;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Gamebase.Sound.NodeCanvas.Actions
{
    [PublicAPI]
    [Name("Resume Sound")]
    [Category("✫ Gamebase/Sound")]
    public sealed class ResumeSound : ActionTask
    {
        [BlackboardOnly] public BBParameter<ISoundPlayer> soundPlayer = default;
        
        protected override void OnExecute()
        {
            soundPlayer.value.Resume();
            EndAction(true);
        }
    }
}
#endif