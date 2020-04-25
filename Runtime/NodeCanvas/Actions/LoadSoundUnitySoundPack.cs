using Gamebase.Sound.Unity;
using JetBrains.Annotations;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UniRx.Async;
using UnityEngine;

namespace Gamebase.Sound.NodeCanvas.Actions
{
    [PublicAPI]
    [Name("Load Sound (UnitySoundPack)")]
    [Category("✫ Gamebase/Sound")]
    public sealed class LoadSoundUnitySoundPack : ActionTask<SoundTaskManager>
    {
        [RequiredField] public UnitySoundPackReference reference = default;

        [BlackboardOnly] public BBParameter<ISoundPlayer> saveAs = default;
        
        protected override void OnExecute()
        {
            Load().Forget();
        }

        private async UniTask Load()
        {
            saveAs.value = await agent.Manager.Load(reference.RuntimeKey.ToString());
            EndAction(true);
        }
    }
}