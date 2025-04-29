using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.Editor.AutoUI
{

    // 仅仅生成一个树状框架，执行rectTransform中的信息
    public class AutoUIFrameworkProcesser
    {

        public static GameObject CreateCanvasWithData(Layer layers)
        {
            // 创建Canvas
            GameObject canvasObj = new GameObject("UICanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            switch (layers.canvasLayerData.renderMode)
            {
                case "camera":
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    break;
                case "overlay":
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    break;
                case "worldSpace":
                    canvas.renderMode = RenderMode.WorldSpace;
                    break;
                default:
                    var err = new AutoUIException("遇到无法解析的renderMode:" + layers.canvasLayerData.renderMode);
                    LogUtil.LogError(err);
                    break;
            }
            canvas.renderMode = RenderMode.WorldSpace;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            if (layers.eLayerKind != ELayerKind.canvas || layers.canvasLayerData == null)
            {
                throw new AutoUIException("传入了错误的json数据或者解析时错误");
            }

            // 设置Canvas大小
            UnityEngine.RectTransform canvasRect = canvasObj.GetComponent<UnityEngine.RectTransform>();
            canvasRect.sizeDelta = new Vector2(layers.canvasLayerData.width, layers.canvasLayerData.height);

            // 处理所有层
            ProcessFrameWork(layers.layers, canvasObj.transform);

            return canvasObj;
        }
        public static void ProcessFrameWork(List<Layer> layers, Transform parentTransform)
        {
            foreach (var layer in layers)
            {
                GameObject layerGameObject = new GameObject(layer.name);
                layerGameObject.transform.SetParent(parentTransform);
                UnityEngine.RectTransform rectTransform = layerGameObject.AddComponent<UnityEngine.RectTransform>();
                // 处理rectTransform
                if (layer.rectTransform != null)
                {
                    AutoUIRectTransformProcessor.RectTransformProcessor(ref rectTransform,in layer.rectTransform);
                }
                else
                {
                    AutoUIException err = new("出现错误,遇到rectTransform为null的情况");
                    LogUtil.LogError(err);
                }

                if (layer.eLayerKind == ELayerKind.group)
                {
                    // 在PS中为Group意味着下面还有东西，并且由于group图层没有任何图片文字信息所以除了rectTransfrom之外不需要处理
                    ProcessFrameWork(layer.layers, layerGameObject.transform);
                }
                else
                {
                    // 在PS中不是Group可以认为不再需要遍历往下了
                    // 框架部分仅仅对rectTransform进行处理
                    AutoUILayersProcessor.LayerProcessor(in layer,ref layerGameObject);
                }
            }
        }





    }


}

