#if GAMEBASE_ADD_MASTERAUDIO
using System;
using DarkTonic.MasterAudio;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Gamebase.Sound.MasterAudio
{
    [Serializable]
    public sealed class DynamicSoundGroupCreatorReference : AssetReferenceT<DynamicSoundGroupCreator>
    {
        public DynamicSoundGroupCreatorReference(string guid) : base(guid)
        {
        }

        public override bool ValidateAsset(Object obj)
        {
            var gameObject = obj as GameObject;
            if (gameObject == null)
                return false;

            return gameObject.GetComponent<DynamicSoundGroupCreator>() != null;
        }

        public override bool ValidateAsset(string path)
        {
            return path.EndsWith(".prefab");
        }
    }
}
#endif