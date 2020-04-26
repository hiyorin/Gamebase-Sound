#if GAMEBASE_ADD_ADX2 && GAMEBASE_ADD_NODECANVAS
using Gamebase.Sound.NodeCanvas;
using JetBrains.Annotations;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UniRx.Async;

namespace Gamebase.Sound.Adx2.NodeCanvas.Actions
{
    [PublicAPI]
    [Name("Load Sound (AcbAsset)")]
    [Category("✫ Gamebase/Sound")]
    public sealed class LoadSoundAcbAsset : ActionTask<SoundTaskManager>
    {
        [RequiredField] public AcbAssetReference reference = default;

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