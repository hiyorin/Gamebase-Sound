#if GAMEBASE_ADD_ADX2
using System;
using System.IO;
using System.Linq;
using Gamebase.Loader.Asset;
using Gamebase.Utilities;
using UniRx.Async;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gamebase.Sound.Adx2.Internal
{
    internal sealed class Adx2SoundLoader
    {
        private readonly IAssetLoader assetLoader;
        
        private readonly CacheDirectory cache;

        public Adx2SoundLoader(IAssetLoader assetLoader, CacheDirectory cache)
        {
            this.assetLoader = assetLoader;
            this.cache = cache;
        }

        public async UniTask<CriAtomCueSheet> Load(object key)
        {
            if (key == default)
                throw new ArgumentNullException();

            // acb
            var acbAsset = await assetLoader.Load<TextAsset>(key);
            var cueSheetName = Path.GetFileName(acbAsset.Result.name);
            var cueSheet = CriAtom.GetCueSheet(cueSheetName);
            if (cueSheet != default)
            {
                assetLoader.Unload(acbAsset);
                return cueSheet;
            }
            var acbBytes = acbAsset.Result.bytes;
            
            // awb
            var awbAssetName = Path.ChangeExtension(acbAsset.Result.name, "awb");
            var awbFilePath = await LoadAwb(awbAssetName);
            assetLoader.Unload(acbAsset);

            // cue sheet
            cueSheet = CriAtom.AddCueSheetAsync(cueSheetName, acbBytes, awbFilePath);
            await UniTask.WaitUntil(() => !cueSheet.IsLoading);
            return cueSheet;
        }

        public void Unload(CriAtomCueSheet cueSheet)
        {
            if (cueSheet == default)
                throw new ArgumentNullException();
            
            CriAtom.RemoveCueSheet(cueSheet.name);
        }
        
        private async UniTask<string> LoadAwb(object key)
        {
            if (AddressableUtility.FindAssetBundlePath(key, out var downloadFiles))
            {
                var srcPath = downloadFiles.FirstOrDefault();
                var srcFileName = Path.GetFileName(srcPath);

                var dstPath = cache.FullPath(srcFileName);
                if (!cache.Exists(srcFileName))
                {
                    var handle = await assetLoader.Load<TextAsset>(key);
                    await cache.SaveAsync(srcFileName, handle.Result.bytes);
                    Addressables.Release(handle);
                }

                return dstPath;
            }

            return null;
        }
    }
}
#endif