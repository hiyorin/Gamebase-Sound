#if false
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Gamebase.Sound.Editor
{
    internal sealed class SoundMonitorWindow : EditorWindow
    {
        [MenuItem("Tools/Gamebase/Sound Monitor")]
        private static void Open()
        {
            var window = GetWindow<SoundMonitorWindow>();
            window.titleContent = new GUIContent("Sound Monitor");
        }

        private ISoundManager soundManager;
        
        private IList<string> cacheInfos = new List<string>();
        
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
        
        private void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    SearchComponents();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    soundManager = null;
                    break;
            }   
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SearchComponents();
        }
        
        private void SearchComponents()
        {
            foreach (var context in FindObjectsOfType<SceneContext>())
            {
                soundManager = context.Container.TryResolve<ISoundManager>();
                if (soundManager != null)
                    break;
            }
        }
        
        private void OnGUI()
        {
            if (soundManager != null)
            {
                cacheInfos.Clear();
                soundManager.GetCacheInfos(ref cacheInfos);

                foreach (var cacheInfo in cacheInfos)
                {
                    var lines = cacheInfo.Split('\n');
                    EditorGUILayout.LabelField(lines[0]);
                    EditorGUI.indentLevel++;
                    for (var i = 1; i < lines.Length; i++)
                    {
                        var keyValue = lines[i].Split(':');
                        if (keyValue.Length >= 2)
                            EditorGUILayout.TextField(keyValue[0].Trim(), keyValue[1].Trim());
                        else if (!string.IsNullOrEmpty(keyValue[0]))
                            EditorGUILayout.TextField(keyValue[0].Trim());
                        else
                            EditorGUILayout.Space();
                    }
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                EditorGUILayout.LabelField("Not found.");
            }
        }
    }
}
#endif