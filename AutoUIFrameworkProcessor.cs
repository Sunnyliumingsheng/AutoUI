using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityFramework;

namespace Assets.Scripts.Tools.Editor.AutoUI
{

    // 进行初步处理，得到一个大概的外观和框架
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
                    LogUtil.LogError("遇到无法解析的renderMode:" + layers.canvasLayerData.renderMode);
                    break;
            }
            canvas.renderMode = RenderMode.WorldSpace;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            if (layers.eLayerKind != ELayerKind.canvas || layers.canvasLayerData == null)
            {
                LogUtil.LogError("出现了错误,遇到了无法解析的layer类型或者canvasLayerData为null");
            }

            // 设置Canvas大小
            UnityEngine.RectTransform canvasRect = canvasObj.GetComponent<UnityEngine.RectTransform>();
            canvasRect.sizeDelta = new Vector2(layers.canvasLayerData.width, layers.canvasLayerData.height);
            var prefabPath = AutoUIFile.SavePrefabAndCleanup(canvasObj);
            canvasObj = AutoUIUtil.OpenPrefabByPath(prefabPath);


            return canvasObj;
        }


        // 生成一个GameObject并处理默认的rectTransform信息
        public static GameObject ProcessLayerFramework(in Layer layer, ref GameObject parent)
        {
            Transform parentTransform = parent.transform;
            GameObject layerGameObject = new GameObject(layer.name);
            layerGameObject.transform.SetParent(parentTransform);
            UnityEngine.RectTransform rectTransform = layerGameObject.AddComponent<UnityEngine.RectTransform>();
            // 处理rectTransform
            if (layer.rectTransform != null)
            {
                AutoUIRectTransformProcessor.RectTransformProcessor(ref rectTransform, in layer.rectTransform);
            }
            else
            {
                LogUtil.LogError("出现错误,遇到rectTransform为null的情况");
            }
            return layerGameObject;
        }

        // 初始化处理预制体的各个层级
        public static void 递归处理所有图层(in List<Layer> layers, ref GameObject parentGameObject)
        {
            foreach (var layer in layers)
            {
                GameObject newGameObject = ProcessLayerFramework(in layer, ref parentGameObject);
                switch (layer.eLayerKind)
                {
                    case ELayerKind.group:
                        递归处理所有图层(in layer.layers, ref newGameObject);
                        break;
                    case ELayerKind.pixel:
                        AutoUIPixelLayerProcessor.PixelLayerProcessor(in layer, ref newGameObject);
                        break;
                    case ELayerKind.text:
                        AutoUITextLayerProcessor.TextLayerProcessor(in layer, ref newGameObject);
                        break;
                    default:
                        LogUtil.LogWarning("初始化生成预制体时出现了无法解析的layer类型:" + layer.eLayerKind);
                        break;
                }
                if (layer.components != null)
                {
                    foreach (var component in layer.components)
                    {
                        LogUtil.Log(component.ToString());
                    }
                }
            }
        }

        
        


    }


}

