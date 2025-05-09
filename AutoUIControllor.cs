using System.Collections.Generic;
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
                else
                {
                    LayerProcessor(layer, layerGameObject);
                }

            }
        }
        private static void LayerProcessor(Layer layer, GameObject layerGameObject)
        {
            switch (layer.eLayerKind)
            {
                case ELayerKind.pixel:
                    var task=AutoUI.MainThread.AsyncRun<string[]>(() =>
                    {
                        return AutoUILayersProcessor.PixelLayerProcessor0(layer, layerGameObject);
                    });
                    string[] paths = task.Result;
                    if (paths!=null){
                        ProductBase gui= GUIManager.CreateGUINotSelectSprite();
                        gui.PutOnLine1();
                        AutoUIEventManager.GUINotSelectSpriteEvent.Subscribe(AutoUILayersProcessor.OnGUINotSelectSpriteEvent);
                    }
                    break;
                    // case ELayerKind.pixel:
                    //     PixelLayerProcessor(in layer, ref layerGameObject);
                    //     break;
                    // case ELayerKind.text:
                    //     TextLayerProcessor(in layer, ref layerGameObject);
                    //     break;
                    // default:
                    //     AutoUIException err = new AutoUIException("逻辑错误，这里不应该有其他图层进入" + layer.eLayerKind);
                    //     LogUtil.LogError(err);
                    //     break;
            }
        }

    }

}