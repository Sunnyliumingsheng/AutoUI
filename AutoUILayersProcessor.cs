
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    // 对每个层级进行一个简单的处理
    public class AutoUILayersProcessor
    {
        public static void LayerProcessor(in Layer layer, ref GameObject layerGameObject)
        {
            switch (layer.eLayerKind)
            {
                case ELayerKind.smartObject:
                    SmartObjectLayerProcessor(in layer, ref layerGameObject);
                    break;
                case ELayerKind.pixel:
                    PixelLayerProcessor(in layer, ref layerGameObject);
                    break;
                case ELayerKind.text:
                    TextLayerProcessor(in layer, ref layerGameObject);
                    break;
                default:
                    AutoUIException err = new AutoUIException("逻辑错误，这里不应该有其他图层进入" + layer.eLayerKind);
                    LogUtil.LogError(err);
                    break;
            }
        }


        public static void SmartObjectLayerProcessor(in Layer layer, ref GameObject layerGameObject)
        {
            // 智能对象的图片依赖引用名称
            string imageAssetPath= AutoUIImagesImportProcessor.ParseImageName(layer.smartObjectLayerData.fileReference);
            if(!AutoUIImagesImportProcessor.IsImageExist(imageAssetPath)){
                AutoUIException err = new AutoUIException("智能对象的图片依赖引用名称不存在 fileReference:"+layer.smartObjectLayerData.fileReference+"   imageAssetPath:"+imageAssetPath);
                LogUtil.LogError(err);
                return;
            }
            Sprite sprite = Resources.Load<Sprite>(imageAssetPath);
            // 添加image
            Image image = layerGameObject.AddComponent<Image>();
            image.sprite = sprite;
        }

        public static void PixelLayerProcessor(in Layer layer, ref GameObject layerGameObject)
        {
            string imageAssetPath= AutoUIImagesImportProcessor.ParseImageName(layer.name)+".png";
            if(!AutoUIImagesImportProcessor.IsImageExist(imageAssetPath)){
                AutoUIException err = new AutoUIException("像素图层的图片依赖引用名称不存在name:"+layer.name+"   imageAssetPath:"+imageAssetPath);
                LogUtil.LogError(err);
                return; 
            }
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(imageAssetPath);
            if(sprite == null){
                AutoUIException err = new AutoUIException("找不到这个sprite  路径为:"+imageAssetPath);
                LogUtil.LogError(err);
                return;
            }
            // 添加image
            Image image = layerGameObject.AddComponent<Image>();
            image.sprite = sprite;
        }
        public static void TextLayerProcessor(in Layer layer, ref GameObject layerGameObject)
        {
            // 第一部分，添加TMP
            TextMeshProUGUI tmp = layerGameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = layer.textLayerData.text;
            // todo : 寻找到一个合适的字体转化关系函数。并进行使用
            tmp.fontSize = AutoUIUtil.PSTextSizeToUnityTMPFontSize(Mathf.RoundToInt(layer.textLayerData.fontSize));
            TMP_FontAsset tmpFontAsset = Resources.Load<TMPro.TMP_FontAsset>(AutoUIConfig.config.text.fontAssetPath);
            tmp.font = tmpFontAsset;
            tmp.color = new Color(
                layer.textLayerData.color.r / 255f,
                layer.textLayerData.color.g / 255f,
                layer.textLayerData.color.b / 255f
                );
            
        }
        



    }
}