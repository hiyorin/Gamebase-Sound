using JetBrains.Annotations;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Gamebase.Sound.NodeCanvas.Actions
{
    [PublicAPI]
    [Name("Play Sound")]
    [Category("✫ Gamebase/Sound")]
    public sealed class PlaySound : ActionTask
    {
        [BlackboardOnly] public BBParameter<ISoundPlayer> soundPlayer = default;

        [RequiredField] public BBParameter<string> soundName = default;
        
        public BBParameter<bool> loop = default;
        
        protected override void OnExecute()
        {
            soundPlayer.value.Play(soundName.value, loop.value);
            EndAction(true);
        }
    }
}