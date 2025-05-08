using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class AutoUIUtil
    {
        public static string scenePath = "Assets/Scenes/Start.unity";

        public static float PSTextSizeToUnityTMPFontSize(int psTextSize)
        {
            // 这是一个经验系数，请随时调整
            return psTextSize * AutoUIConfig.config.text.fontScale;
        }
        // 打开scene，并进入到预制体中
        public static GameObject OpenPrefabByPath(string prefabPath)
        {
            EditorSceneManager.OpenScene(scenePath);
            var prefab = PrefabStageUtility.OpenPrefab(prefabPath);
            if (prefab != null)
            {
                EditorWindow.FocusWindowIfItsOpen<SceneView>();
                Selection.activeObject = prefab;
            }
            else
            {
                new AutoUIException("打开prefab失败");
            }
            return prefab.prefabContentsRoot;
        }
    }
}