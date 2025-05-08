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
        public static void ShowWindow()
        {
            GetWindow<AutoUI>("AutoUI 交互面板");
            AutoUIMain();
        }
        private void OnGUI()
        {
            AutoUIInteractive.Interactice();
        }
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
                /*
                爷不用这种方式导入图片了，如果要用，只能在新项目立项的时候，规定好图片名称和路径的映射关系，或者图片都放到一个文件夹中，才能重新启用。
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
                */
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
                LogUtil.Log("===  ===");// 保存预制体的功能已经不再需要单独一步了。
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
                LogUtil.Log("=== 收尾工作 ===");
                try
                {
                    LogUtil.Hint();
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
            }
        }

    }


}