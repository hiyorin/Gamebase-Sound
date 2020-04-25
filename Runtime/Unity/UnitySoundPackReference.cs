using System;
using UnityEngine.AddressableAssets;

namespace Gamebase.Sound.Unity
{
    [Serializable]
    public sealed class UnitySoundPackReference : AssetReferenceT<UnitySoundPack>
    {
        public UnitySoundPackReference(string guid) : base(guid)
        {
        }
    }
}