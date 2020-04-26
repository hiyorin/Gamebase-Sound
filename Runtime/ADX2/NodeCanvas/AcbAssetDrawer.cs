#if GAMEBASE_ADD_ADX2 && GAMEBASE_ADD_NODECANVAS && UNITY_EDITOR
using JetBrains.Annotations;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace Gamebase.Sound.Adx2.NodeCanvas
{
    [PublicAPI]
    public sealed class AcbAssetDrawer : ObjectDrawer<AcbAssetReference>
    {
        private sealed class Dummy : ScriptableObject
        {
            public AcbAssetReference Reference;
        }
        
        public override AcbAssetReference OnGUI(GUIContent content, AcbAssetReference instance)
        {
            var dummy = ScriptableObject.CreateInstance<Dummy>();
            dummy.Reference = instance;
            var so = new SerializedObject(dummy);
            var sp = so.FindProperty("Reference");
            EditorGUILayout.PropertyField(sp, true);
            so.ApplyModifiedProperties();
            return dummy.Reference;
        }
    }
}
#endif