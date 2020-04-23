#if GAMEBASE_ADD_ADX2
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gamebase.Loader.Asset;
using Gamebase.Utilities;
using UniRx.Async;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gamebase.Sound.Adx2
{
    public sealed class Adx2SoundLoader
    {
        private readonly IAssetLoader assetLoader;
        
        private readonly CacheDirectory cache;

        private TextAsset acbAsset;

        private TextAsset awbAsset;

        public Adx2SoundLoader(IAssetLoader assetLoader, CacheDirectory cache)
        {
            this.assetLoader = assetLoader;
            this.cache = cache;
        }
        
        public async UniTask<CriAtomCueSheet> Load(string cueSheetName, object acbKey, object awbKey)
        {
            if (string.IsNullOrEmpty(cueSheetName) || acbKey == null)
                throw new ArgumentNullException();

            // CriAtomに追加済みなら再利用
            CriAtomCueSheet cueSheet = CriAtom.GetCueSheet(cueSheetName);
            if (cueSheet != null)
                return cueSheet;
            
            byte[] acbBytes = await LoadAcb(acbKey);

            string awbFilePath = null;
            if (awbKey != null)
            {
                awbFilePath = await LoadAwb(awbKey);
            }

            cueSheet = CriAtom.AddCueSheetAsync(cueSheetName, acbBytes, awbFilePath);
            await UniTask.WaitUntil(() => !cueSheet.IsLoading);

            return cueSheet;
        }

        private async UniTask<byte[]> LoadAcb(object key)
        {
            var handle = await assetLoader.Load<TextAsset>(key);
            byte[] acbBytes = handle.Result.bytes;
            assetLoader.Unload(handle);
            return acbBytes;
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