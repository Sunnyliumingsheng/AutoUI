using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    // 这是一个对json数据进行递归处理和负责中断的线程。可以说是核心逻辑

    public class AutoUIControllor
    {

        // 原来你才是最终boss？！
        public static void MainControllor(Layer layers, GameObject canvasObj)
        {
            // 开始进行遍历
            SafeExit();
            RecursiveProcess(layers.layers, canvasObj);
        }
        private static void RecursiveProcess(List<Layer> layers, GameObject parent)
        {
            foreach (var layer in layers)
            {
                var task = AutoUI.MainThread.AsyncRun<GameObject>(() =>
                {
                    return AutoUIFrameworkProcesser.ProcessLayerFramework(in layer, ref parent);
                });
                GameObject layerGameObject = task.Result;
                if (layer.eLayerKind == ELayerKind.group)
                {
                    // 在PS中为Group意味着下面还有东西，并且由于group图层没有任何图片文字信息所以除了rectTransfrom之外不需要处理
                    RecursiveProcess(layer.layers, layerGameObject);
                }
                if (layer.eLayerKind == ELayerKind.pixel)
                {
                    AutoUILayersProcessor.PixelLayerProcessor(layer, layerGameObject);
                }

            }
        }
        public static void SafeExit()
        {
            AutoUIEventManager.GUISafeExitEvent.Subscribe((sender, args) =>
            {
                AutoUI.controllor.Abort();
                UnityEditor.EditorApplication.ExitPlaymode();
            });
        }


    }

}