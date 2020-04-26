#if GAMEBASE_ADD_MASTERAUDIO && GAMEBASE_ADD_NODECANVAS
using Gamebase.Sound.NodeCanvas;
using JetBrains.Annotations;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UniRx.Async;

namespace Gamebase.Sound.MasterAudio.NodeCanvas.Actions
{
    [PublicAPI]
    [Name("Load Sound (DynamicSoundGroupCreator)")]
    [Category("✫ Gamebase/Sound")]
    public sealed class LoadSoundDynamicSoundGroupCreator : ActionTask<SoundTaskManager>
    {
        [RequiredField] public DynamicSoundGroupCreatorReference reference = default;

        [BlackboardOnly] public BBParameter<ISoundPlayer> saveAs = default;

        protected override string info => $"{base.info} - {reference.editorAsset.name}";

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
#endif