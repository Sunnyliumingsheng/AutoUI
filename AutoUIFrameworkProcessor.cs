using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityFramework;

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
        public static void InitLayerProcessor(in List<Layer> layers, ref GameObject parentGameObject)
        {
            foreach (var layer in layers)
            {
                GameObject newGameObject = ProcessLayerFramework(in layer, ref parentGameObject);
                switch (layer.eLayerKind)
                {
                    case ELayerKind.group:
                        InitLayerProcessor(in layer.layers, ref newGameObject);
                        break;
                    case ELayerKind.pixel:
                        PixelLayerProcessor(in layer, ref newGameObject);
                        break;
                    case ELayerKind.text:
                        TextLayerProcessor(in layer, ref newGameObject);
                        break;
                    default:
                        LogUtil.LogWarning("初始化生成预制体时出现了无法解析的layer类型:" + layer.eLayerKind);
                        break;
                }

            }
        }

        public static void PixelLayerProcessor(in Layer layer, ref GameObject pixelGameObject)
        {
            FindSpriteResult result = AutoUIAssets.GetSprite(layer.name);
            if (result == null)
            {
                LogUtil.LogError("无法找到对应的sprite:" + layer.name);
                return;
            }
            switch (result.status)
            {
                case EFindAssetStatus.oneResult:
                    Sprite sprite = result.oneResult.sprite;
                    AutoUIPixelTool.PixelLayerGameObjectAddSprite(pixelGameObject, sprite);
                    break;
                case EFindAssetStatus.manyResult:
                    LogUtil.LogWarning("出现了多个同名的sprite:" + layer.name + "需要手动解决");
                    break;
                case EFindAssetStatus.cantFind:
                    LogUtil.LogWarning("没有找到对应的sprite:" + layer.name);
                    break;
                default:
                    LogUtil.LogError("出现了无法解析的EFIndAssetStatus:" + result.status);
                    break;
            }
        }
        public static void TextLayerProcessor(in Layer layer, ref GameObject textGameObject)
        {
            // 第一部分，添加TMP
                TextMeshProUGUI tmp = textGameObject.AddComponent<TextMeshProUGUI>();
                tmp.text = layer.textLayerData.text;
                // todo : 寻找到一个合适的字体转化关系函数。并进行使用
                tmp.fontSize = AutoUIUtil.PSTextSizeToUnityTMPFontSize(Mathf.RoundToInt(layer.textLayerData.fontSize));
                TMP_FontAsset tmpFontAsset = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(AutoUIConfig.config.text.fontAssetPath);
                if (tmpFontAsset == null)
                {
                    LogUtil.LogError("找不到字体资源 路径为:" + AutoUIConfig.config.text.fontAssetPath);
                    return;
                }
                tmp.font = tmpFontAsset;
                tmp.color = new Color(
                    layer.textLayerData.color.r / 255f,
                    layer.textLayerData.color.g / 255f,
                    layer.textLayerData.color.b / 255f
                    );
                var localizationTextTMP = textGameObject.AddComponent<LocalizationText_TMP>();
                // 使用反射来给私有成员mLabel赋值
                var field = typeof(LocalizationText_TMP).GetField("mLabel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                field.SetValue(localizationTextTMP, tmp);

                // mywar特殊支持
                if (layer.textLayerData.haveShadow)
                {
                    Material presetMaterial = AssetDatabase.LoadAssetAtPath<Material>(AutoUIConfig.config.fontMaterialPath.miaobian);
                    if (presetMaterial == null)
                    {
                        LogUtil.LogError("找不到预设材质 路径为:" + AutoUIConfig.config.fontMaterialPath.miaobian);
                        return;
                    }
                    LogUtil.Log(presetMaterial.name);
                    tmp.fontSharedMaterial = presetMaterial;
                }
                tmp.enableWordWrapping = false;
                EditorUtility.SetDirty(localizationTextTMP);
        }


    }


}

