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
        public static Thread controllor;
        public static bool isRunning = false;
        public static GameObject prefabGameObject;
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
        void OnDisable()
        {
            LogUtil.Log("手动关闭窗口");
            AutoUIControllor.exit();
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
                    AutoUIFrameworkProcesser.InitLayerProcessor(in layers.layers, ref prefabGameObject);
                    AutoUIFile.SavePrefabAndCleanup(prefabGameObject);
                    LogUtil.Log("创建预制体成功");
                }
                catch (Exception err)
                {
                    LogUtil.HandleAutoUIError(err);
                    return;
                }
                LogUtil.Log("=== 新开一个线程作为控制器开始对预制体进行修改 ===");
                try
                {
                    // 新建一个线程
                    isRunning = true;
                    controllor = new Thread(() =>
                   {
                       try
                       {
                           LogUtil.Log("hello , 成功开启了一个线程");
                           AutoUIControllor.MainControllor(prefabGameObject);
                           LogUtil.Log("成功退出了线程");
                           // 重新保存一下这个prefab
                           MainThread.Run(() =>
                           {
                               AutoUIFile.SavePrefabAndCleanup(prefabGameObject);
                           });
                           LogUtil.Log("重新保存prefab");
                       }
                       catch (Exception err)
                       {
                           MainThread.Run(() =>
                           {
                               LogUtil.HandleAutoUIError(err);
                           });
                           return;
                       }
                   });
                    controllor.Start();
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