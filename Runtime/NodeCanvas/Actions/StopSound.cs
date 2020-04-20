using JetBrains.Annotations;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Gamebase.Sound.NodeCanvas.Actions
{
    [PublicAPI]
    [Name("Stop Sound")]
    [Category("✫ Gamebase/Sound")]
    public sealed class StopSound : ActionTask
    {
        [BlackboardOnly] public BBParameter<ISoundPlayer> soundPlayer = default;
        
        protected override void OnExecute()
        {
            soundPlayer.value.Stop();
            EndAction(true);
        }
    }
}