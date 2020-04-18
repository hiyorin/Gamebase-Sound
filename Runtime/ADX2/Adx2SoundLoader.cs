#if GAMEBASE_ADD_ADX2
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gamebase.Utilities;
using UniRx.Async;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gamebase.Sound.Adx2
{
    public sealed class Adx2SoundLoader
    {
        private readonly CacheDirectory cache;

        private TextAsset acbAsset;

        private TextAsset awbAsset;

        public Adx2SoundLoader(CacheDirectory cache)
        {
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
            var operation = Addressables.LoadAssetAsync<TextAsset>(key);
            await operation.Task;
            if (!operation.IsValid())
            {
                Debug.unityLogger.LogError(nameof(Adx2SoundLoader), "Load acb Error.");
                throw operation.OperationException;
            }
            
            byte[] acbBytes = operation.Result.bytes;
            Addressables.Release(operation);

            return acbBytes;
        }

        private async UniTask<string> LoadAwb(object key)
        {
            IList<string> downloadFiles;
            if (AddressableUtility.FindAssetBundlePath(key, out downloadFiles))
            {
                var srcPath = downloadFiles.FirstOrDefault();
                var srcFileName = Path.GetFileName(srcPath);

                var dstPath = cache.FullPath(srcFileName);
                if (!cache.Exists(srcFileName))
                {
                    var operation = Addressables.LoadAssetAsync<TextAsset>(key);
                    await operation.Task;
                    if (operation.IsValid())
                    {
                        await cache.SaveAsync(srcFileName, operation.Result.bytes);
                        Addressables.Release(operation);
                    }
                }

                return dstPath;
            }

            return null;
        }
    }
}
#endif