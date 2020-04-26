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

        private ISoundVolumeController volumeController;
        
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
                    volumeController = null;
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
                if (soundManager == null)
                    soundManager = context.Container.TryResolve<ISoundManager>();

                if (volumeController == null)
                    volumeController = context.Container.TryResolve<ISoundVolumeController>();
                
                if (soundManager != null && volumeController != null)
                    break;
            }
        }

        private void OnGUI()
        {
            if (volumeController != null)
            {
                EditorGUILayout.LabelField("Volume");
                EditorGUI.indentLevel++;
                volumeController.MasterVolume = EditorGUILayout.Slider("Master", volumeController.MasterVolume, 0.0f, 1.0f);
                volumeController.BgmVolume = EditorGUILayout.Slider("BGM", volumeController.BgmVolume, 0.0f, 1.0f);
                volumeController.SeVolume = EditorGUILayout.Slider("SE", volumeController.SeVolume, 0.0f, 1.0f);
                volumeController.VoiceVolume = EditorGUILayout.Slider("Voice", volumeController.VoiceVolume, 0.0f, 1.0f);
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.LabelField("Not Found ISoundVolumeController.");
            }
            
            EditorGUILayout.Space();

            if (soundManager != null)
            {
                EditorGUILayout.LabelField("Loaded Sounds");
                EditorGUI.indentLevel++;
                foreach (var info in soundManager.GetInfos())
                {
                    EditorGUILayout.LabelField(info);
                }
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.LabelField("Not Found ISoundManager.");
            }
        }
    }
}