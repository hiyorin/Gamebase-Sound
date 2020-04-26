#if GAMEBASE_ADD_MASTERAUDIO && GAMEBASE_ADD_NODECANVAS && UNITY_EDITOR
using JetBrains.Annotations;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

namespace Gamebase.Sound.MasterAudio.NodeCanvas
{
    [PublicAPI]
    public sealed class DynamicSoundGroupCreatorDrawer : ObjectDrawer<DynamicSoundGroupCreatorReference>
    {
        private sealed class Dummy : ScriptableObject
        {
            public DynamicSoundGroupCreatorReference Reference;
        }
        
        public override DynamicSoundGroupCreatorReference OnGUI(GUIContent content, DynamicSoundGroupCreatorReference instance)
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