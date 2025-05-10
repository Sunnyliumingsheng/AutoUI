using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.Editor.AutoUI
{

    // 仅仅生成一个树状框架，执行rectTransform中的信息
    public class AutoUIFrameworkProcesser
    {

/*         这里是原来的代码
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
            var prefabPath= AutoUIFile.SavePrefabAndCleanup(canvasObj);
            canvasObj=AutoUIUtil.OpenPrefabByPath(prefabPath);

            // 处理所有层
            ProcessFrameWork(layers.layers, canvasObj.transform);

            return canvasObj;
        } */
        
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
            var prefabPath= AutoUIFile.SavePrefabAndCleanup(canvasObj);
            canvasObj=AutoUIUtil.OpenPrefabByPath(prefabPath);

            
            return canvasObj;
        }
        
        
        // 生成一个GameObject并处理默认的rectTransform信息
        public static GameObject ProcessLayerFramework(in Layer layer,ref GameObject parent)
        {
                Transform parentTransform=parent.transform;
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
                return layerGameObject;
        }





    }


}

