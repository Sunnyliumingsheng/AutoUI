
using Framework;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityFramework;

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
            Sprite sprite=null;
            FindSpriteResult findSpriteResult = AutoUIAssets.GetSprite(layer.name);
            if (findSpriteResult.status== EFindAssetStatus.cantFind){
                LogUtil.LogWarning("获取Sprite失败:"+layer.name);
                return;
            }
            if (findSpriteResult.status== EFindAssetStatus.manyResult){
                LogUtil.LogWarning("获取到多个Sprite已经默认采用第一个搜索到的Sprite:"+layer.name);
                sprite=findSpriteResult.manyResult[0].sprite;
            }
            if (findSpriteResult.status== EFindAssetStatus.oneResult){
                sprite=findSpriteResult.oneResult.sprite; 
            }
            if (sprite==null){
                LogUtil.LogError("Sprite为空:"+layer.name);
                return; 
            }
            
            // 添加image
            Image image = layerGameObject.AddComponent<Image>();
            image.sprite = sprite;
            if(sprite.border!=Vector4.zero){
                image.type = Image.Type.Sliced;
            }
        }
        public static void TextLayerProcessor(in Layer layer, ref GameObject layerGameObject)
        {
            // 第一部分，添加TMP
            TextMeshProUGUI tmp = layerGameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = layer.textLayerData.text;
            // todo : 寻找到一个合适的字体转化关系函数。并进行使用
            tmp.fontSize = AutoUIUtil.PSTextSizeToUnityTMPFontSize(Mathf.RoundToInt(layer.textLayerData.fontSize));
            TMP_FontAsset tmpFontAsset = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(AutoUIConfig.config.text.fontAssetPath);
            if (tmpFontAsset == null){
                LogUtil.LogError("找不到字体资源 路径为:"+AutoUIConfig.config.text.fontAssetPath);
                return;
            }
            tmp.font = tmpFontAsset;
            tmp.color = new Color(
                layer.textLayerData.color.r / 255f,
                layer.textLayerData.color.g / 255f,
                layer.textLayerData.color.b / 255f
                );
            var localizationTextTMP= layerGameObject.AddComponent<LocalizationText_TMP>();
            // 使用反射来给私有成员mLabel赋值
            var field=typeof(LocalizationText_TMP).GetField("mLabel",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(localizationTextTMP,tmp);
            EditorUtility.SetDirty(localizationTextTMP);

        }
        



    }
}