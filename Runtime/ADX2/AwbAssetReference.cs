#if GAMEBASE_ADD_ADX2
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace Gamebase.Sound.Adx2
{
    [Serializable]
    public sealed class AwbAssetReference : AssetReferenceT<TextAsset>
    {
        public AwbAssetReference(string guid) : base(guid)
        {
            
        }
        
        public override bool ValidateAsset(Object obj)
        {
            return ValidateAsset(obj.name);
        }

        public override bool ValidateAsset(string path)
        {
            return path.IndexOf(".awb", StringComparison.Ordinal) > 0;
        }
    }
}
#endif