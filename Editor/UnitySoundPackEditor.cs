using Gamebase.Sound.Unity;
using UnityEditor;
using UnityEngine;

namespace Gamebase.Sound.Editor
{
    internal sealed class UnitySoundPackEditor
    {
        [MenuItem("Assets/Create/Gamebase/Unity Sound Pack")]
        private static void CreateUnitySoundPack()
        {
            foreach (var selectObject in Selection.objects)
            {
                var selectPath = AssetDatabase.GetAssetPath(selectObject);
                if (!AssetDatabase.IsValidFolder(selectPath))
                    continue;
                
                var instance = ScriptableObject.CreateInstance<UnitySoundPack>();
                AssetDatabase.CreateAsset(instance, $"{selectPath}/{nameof(UnitySoundPack)}.asset");
                AssetDatabase.Refresh();
                break;
            }
        }
    }
}