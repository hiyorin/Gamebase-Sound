using Gamebase.Sound.Unity;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace Gamebase.Sound.NodeCanvas
{
    public class UnitySoundPackDrawer : ObjectDrawer<UnitySoundPackReference>
    {
        private class Dummy : ScriptableObject
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