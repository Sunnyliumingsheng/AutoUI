using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{

    public class AutoUIFile : EditorWindow
    {
        private static string prefabPath = "Assets/Prefabs/UI/test.prefab";
        // 保存一个gameobject为预制体，并返回其路径
        public static string SavePrefabAndCleanup(GameObject target)
        {
            PrefabUtility.SaveAsPrefabAsset(target, prefabPath);
            AssetDatabase.Refresh();
            return prefabPath;
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
        // 选择本地文件夹中的图片，如果没有选择，则返回空字符串
        public static string GUIChooseImagePath()
        {
            string path = EditorUtility.OpenFilePanel("选择图片", "", "png,jpg,jpeg");
            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }
            else{
                return "";
            }
        }


    }








}


