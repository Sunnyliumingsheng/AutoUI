using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{

    public class AutoUIFile : EditorWindow
    {
        private static string prefabPath = "Assets/Prefabs/UI/test.prefab";
        public static void SavePrefabAndCleanup(GameObject target)
        {
            PrefabUtility.SaveAsPrefabAsset(target, prefabPath);
            DestroyImmediate(target);
            AssetDatabase.Refresh();
        }
        public static string SelectFolderPath()
        {
            // 打开文件夹选择对话框
            var selectedFolderPath = EditorUtility.OpenFolderPanel("选择包含data.json的文件夹", "", "");
            LogUtil.Log("选择了文件夹" + selectedFolderPath);
            if (string.IsNullOrEmpty(selectedFolderPath))
            {
                LogUtil.Log("用户取消选择文件夹");
                return "";
            }
            return selectedFolderPath;
        }
        public static bool IsJsonFileExist(string folderPath)
        {
            string jsonPath = folderPath + "/data.json";
            if (!File.Exists(jsonPath))
            {
                var err = new AutoUIException("未找到data.json文件！" + jsonPath);
                LogUtil.LogError(err);
                return false;
            }
            else
            {
                return true;
            }
        }


    }








}


