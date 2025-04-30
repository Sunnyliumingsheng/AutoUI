// 从这个版本开始，脚本就是正式版本，由我阳小帅来编写，并且确保良好的结构。高可扩展性

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    class AutoUI : EditorWindow
    {

        // 选择的文件夹位置
        public static string selectedFolderPath;
        public static string selectedJsonPath;

        public static GameObject prefabGameObjec;
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
                        var err = new AutoUIException("解析终止，因为未选择有效的文件夹路径。");
                        LogUtil.LogError(err);
                        return;
                    }
                    selectedJsonPath = selectedFolderPath + "/data.json";
                    if (!AutoUIFile.IsJsonFileExist(selectedFolderPath))
                    {
                        var err = new AutoUIException("解析终止，因为未选择有效的 JSON 文件路径。");
                        LogUtil.LogError(err);
                        return;
                    }
                    AutoUIConfig.GetAutoUIConfigData();
                    imageNameToSpritePath = new Dictionary<string, string>();
                }
                catch (AutoUIException err)
                {
                    LogUtil.HandleAutoUIError(err);
                    return;
                }
                catch (Exception err)
                {
                    LogUtil.LogError(err);
                    return;
                }
                LogUtil.Log("=== 开始解析json ===");
                try
                {
                    string json = File.ReadAllText(selectedJsonPath);
                    layers = LayerJsonParser.ParseFromJson(json);
                    layers.VerifyLayers();
                }
                catch (AutoUIException err)
                {
                    LogUtil.HandleAutoUIError(err);
                    return;
                }
                catch (Exception err)
                {
                    LogUtil.LogError(err);
                    return;
                }
                LogUtil.Log("=== 导入图片 ===");
                try
                {
                    AutoUIImagesImportProcessor.ImageImportProcessor(selectedFolderPath);
                    LogUtil.Log("导入全部图片");
                }
                catch (AutoUIException err)
                {
                    LogUtil.HandleAutoUIError(err);

                    return;
                }
                catch (Exception err)
                {
                    LogUtil.LogError(err);
                    return;
                }
                LogUtil.Log("=== 开始创建基本框架 ===");
                try
                {
                    prefabGameObjec = AutoUIFrameworkProcesser.CreateCanvasWithData(layers);
                }
                catch (AutoUIException err)
                {
                    LogUtil.HandleAutoUIError(err);

                    return;
                }
                catch (Exception err)
                {
                    LogUtil.LogError(err);
                    return;
                }
                LogUtil.Log("=== 实例化相应的预制体 ===");
                try
                {
                    AutoUIFile.SavePrefabAndCleanup(prefabGameObjec);
                }
                catch (AutoUIException err)
                {
                    LogUtil.HandleAutoUIError(err);

                    return;
                }
                catch (Exception err)
                {
                    LogUtil.LogError(err);
                    return;
                }
                LogUtil.Log("=== 再次遍历并解析和使用组件 ==="); // 实际上，多遍历一次反而是最好的方案。
                try
                {

                }
                catch (AutoUIException err)
                {
                    LogUtil.HandleAutoUIError(err);
                    return;
                }
                catch (Exception err)
                {
                    LogUtil.LogError(err);
                    return;
                }
                LogUtil.Hint();
            }
        }

    }


}