// 从这个版本开始，脚本就是正式版本，由我阳小帅来编写，并且确保良好的结构。高可扩展性

using System;
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
        public static Thread controllor;
        public static GameObject prefabGameObjec;
        public static AutoUIMainThreadDispatcher MainThread;
        public static Layer layers;
        public static Dictionary<string, string> imageNameToSpritePath;

        [MenuItem("Tools/AutoUI")]
        public static void ShowWindow()
        {
            MainThread = AutoUIMainThreadDispatcher.getInstace();
            GetWindow<AutoUI>("AutoUI 交互面板");
            AutoUIMain();
        }
        private void OnGUI()
        {
            AutoUIInteractive.Interactice();
        }
        private void Update()
        {
            while (MainThread.actions.TryDequeue(out Action action))
            {
                action?.Invoke();
            }
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
                LogUtil.Log("=== 加载Sprite ===");
                try
                {
                    AutoUIAssets.InitAssets(layers);
                    LogUtil.Log("加载Sprite");
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
                LogUtil.Log("=== 新开一个线程作为控制器 ===");
                try
                {
                    GameObject canvasObj = AutoUIFrameworkProcesser.CreateCanvasWithData(layers);
                    // 新建一个线程
                    controllor = new Thread(() =>
                   {
                       LogUtil.Log("hello , 成功开启了一个线程");
                       AutoUIControllor.MainControllor(layers, canvasObj);
                   });
                    controllor.Start();
                }
                catch (AutoUIException err)
                {
                    LogUtil.HandleAutoUIError(err);
                    return;
                }
                catch (Exception err)
                {
                    LogUtil.LogError(err);
                }
            }
        }

    }


}