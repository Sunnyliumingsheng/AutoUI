using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class AutoUIUtil
    {
        public static string scenePath = AutoUIConfig.config.Default.Scene.Path;


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
                LogUtil.LogError("打开Prefab失败");
            }
            return prefab.prefabContentsRoot;
        }
        public static void FocusGameObject(GameObject go)
        {
            if (go == null)
            {
                LogUtil.LogError("传入的GameObject为空聚焦失败");
            }
            Selection.activeGameObject = go;
        }

    }
}