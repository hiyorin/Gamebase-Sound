#if GAMEBASE_ADD_NODECANVAS
using JetBrains.Annotations;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Gamebase.Sound.NodeCanvas.Actions
{
    [PublicAPI]
    [Name("Unload Sound")]
    [Category("✫ Gamebase/Sound")]
    public sealed class UnloadSound : ActionTask<SoundTaskManager>
    {
        [RequiredField, BlackboardOnly] public BBParameter<ISoundPlayer> player = default;
        
        protected override void OnExecute()
        {
            agent.Manager.Unload(player.value);
        }
    }
}
#endif