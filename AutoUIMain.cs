// 从这个版本开始，脚本就是正式版本，由我阳小帅来编写，并且确保良好的结构。高可扩展性

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    class AutoUI : EditorWindow
    {

        // 选择的文件夹位置
        public static string selectedFolderPath;
        public static string selectedJsonPath;
        public static GameObject prefabGameObject;
        public static Layer layers;
        public static Dictionary<string, string> imageNameToSpritePath;

        [MenuItem("Tools/AutoUI")]
        public static void AutoUIMain()
        {
            {
                LogUtil.Log("=== AutoUI start ===");
                try
                {
                    LogUtil.ClearLogFile();
                    selectedFolderPath = AutoUIFile.SelectFolderPath();
                    if (string.IsNullOrEmpty(selectedFolderPath))
                    {
                        LogUtil.LogError("解析终止，因为未选择有效的文件夹路径。");
                        return;
                    }
                    selectedJsonPath = selectedFolderPath + "/data.json";
                    if (!AutoUIFile.IsJsonFileExist(selectedFolderPath))
                    {
                        LogUtil.LogError("解析终止，因为未选择有效的 JSON 文件路径。");
                        return;
                    }
                    AutoUIConfig.GetAutoUIConfigData();
                    imageNameToSpritePath = new Dictionary<string, string>();
                }
                catch (Exception err)
                {
                    LogUtil.HandleAutoUIError(err);
                    return;
                }
                LogUtil.Log("=== 开始解析json ===");
                try
                {
                    string json = File.ReadAllText(selectedJsonPath);
                    layers = LayerJsonParser.ParseFromJson(json);
                    layers.VerifyLayers();
                }
                catch (Exception err)
                {
                    LogUtil.HandleAutoUIError(err);
                    return;
                }
                LogUtil.Log("=== 加载Sprite ===");
                try
                {
                    AutoUIAssets.InitAssets(layers);
                    LogUtil.Log("加载Sprite");
                }
                catch (Exception err)
                {
                    LogUtil.HandleAutoUIError(err);
                    return;
                }
                LogUtil.Log("=== 新建一个预制体 ===");
                try
                {
                    prefabGameObject = AutoUIFrameworkProcesser.CreateCanvasWithData(layers);
                    if (prefabGameObject == null)
                    {
                        LogUtil.LogError("创建canvas失败");
                        return;
                    }
                    AutoUIFrameworkProcesser.递归处理所有图层(in layers.layers, ref prefabGameObject);
                    AutoUIFile.SavePrefabAndCleanup(prefabGameObject);
                    LogUtil.Log("创建预制体成功");
                }
                catch (Exception err)
                {
                    LogUtil.HandleAutoUIError(err);
                    return;
                }
                
                LogUtil.Hint();
            }
        }

    }


}