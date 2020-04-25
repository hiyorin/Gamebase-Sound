#if GAMEBASE_ADD_NODECANVAS
using Gamebase.Sound.Unity;
using JetBrains.Annotations;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace Gamebase.Sound.NodeCanvas
{
    [PublicAPI]
    public sealed class UnitySoundPackDrawer : ObjectDrawer<UnitySoundPackReference>
    {
        private sealed class Dummy : ScriptableObject
        {
            public UnitySoundPackReference Reference;
        }
        
        public override UnitySoundPackReference OnGUI(GUIContent content, UnitySoundPackReference instance)
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