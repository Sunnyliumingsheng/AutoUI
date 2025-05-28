using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{

    public class AutoUIFile : EditorWindow
    {
        // 保存一个gameobject为预制体，并返回其路径
        public static string SavePrefabAndCleanup(GameObject target)
        {
            string prefabPath = AutoUIConfig.config.Default.Prefab.Path + "/" + AutoUIConfig.config.Default.Prefab.Name;
            PrefabUtility.SaveAsPrefabAsset(target,prefabPath);
            AssetDatabase.Refresh();
            return prefabPath;
        }
        public static string SavePrefabAndCleanup(GameObject target, string name)
        {
            string prefabPath = AutoUIConfig.config.Default.Prefab.Path + "/" + name+".prefab";
            PrefabUtility.SaveAsPrefabAsset(target, prefabPath);
            AssetDatabase.Refresh();
            return prefabPath;
        }
        public static string SavePrefabAndConnect(GameObject target, string name)
        {
            string prefabPath = AutoUIConfig.config.Default.Prefab.Path + "/" + name + ".prefab";
            PrefabUtility.SaveAsPrefabAssetAndConnect(target, prefabPath, InteractionMode.AutomatedAction);
            AssetDatabase.Refresh();
            return prefabPath;
        }
        public static GameObject LoadPrefab(string prefabName)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AutoUIConfig.config.Default.Prefab.Path + "/" + prefabName + ".prefab");
            if (prefab == null)
            {
                LogUtil.LogError("未找到名为" + prefabName + "的预制体");
                return null;
            }
            return prefab;
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
                LogUtil.LogError("未找到data.json文件！" + jsonPath);
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


