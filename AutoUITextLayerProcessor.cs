using TMPro;
using UnityEditor;
using UnityEngine;

namespace AutoUI
{
    public class AutoUITextLayerProcessor
    {
        public static void TextLayerProcessor(in Layer layer, ref GameObject textGameObject)
        {
            ///////// 基础设置
            TextMeshProUGUI tmp = textGameObject.AddComponent<TextMeshProUGUI>();

            // 默认对齐方式
            tmp.alignment = TextAlignmentOptions.Center;
            // 文本
            tmp.text = layer.textLayerData.text;

            // 字体大小
            tmp.fontSize = Mathf.RoundToInt(CorrectSizeValue(layer.textLayerData.fontSize));

            // 字体资源
            TMP_FontAsset tmpFontAsset = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(AutoUIConfig.config.FontAssets.Default.Path);
            if (tmpFontAsset == null)
            {
                LogUtil.LogError("找不到字体资源 路径为:" + AutoUIConfig.config.FontAssets.Default.Path);
                return;
            }
            tmp.font = tmpFontAsset;

            // 字体颜色
            tmp.color = new Color(
                layer.textLayerData.color.r / 255f,
                layer.textLayerData.color.g / 255f,
                layer.textLayerData.color.b / 255f
                );

            // 描边支持
            if (layer.textLayerData.haveShadow)
            {
                Material presetMaterial = AssetDatabase.LoadAssetAtPath<Material>(AutoUIConfig.config.FontAssets.Default.MaterialPreset.Shadow.Path);
                if (presetMaterial == null)
                {
                    LogUtil.LogError("找不到预设材质 路径为:" + AutoUIConfig.config.FontAssets.Default.MaterialPreset.Shadow.Path);
                    return;
                }
                tmp.fontSharedMaterial = presetMaterial;
            }

            tmp.enableWordWrapping = layer.textLayerData.warp;
            if (layer.textLayerData.warp)
            {
                tmp.alignment = TextAlignmentOptions.TopLeft;
            }

            // 文本的旋转
            tmp.rectTransform.rotation = Quaternion.Euler(0, 0, layer.textLayerData.rotation);

            // 一般来说文本的旋转如果是0那么肯定就需要文本居中，此类文字基本都是以tips的形式出现
            if (layer.textLayerData.rotation != 0)
            {
                tmp.alignment = TextAlignmentOptions.Center;
            }


            /////// 添加组件


            // 本地化组件支持 项目强制
            if (AutoUIConfig.config.Default.Localization.IsUseLocalization)
            {
            }

            // title组件支持
            if (AutoUIUtil.IsComponentExist(in layer, "title"))
            {
                tmpFontAsset = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(AutoUIConfig.config.FontAssets.Title.Path);
                if (tmpFontAsset == null)
                {
                    LogUtil.LogError("找不到字体资源 路径为:" + AutoUIConfig.config.FontAssets.Title.Path);
                    return;
                }
                tmp.font = tmpFontAsset;
                // 描边支持
                if (layer.textLayerData.haveShadow)
                {
                    Material presetMaterial = AssetDatabase.LoadAssetAtPath<Material>(AutoUIConfig.config.FontAssets.Title.MaterialPreset.Shadow.Path);
                    if (presetMaterial == null)
                    {
                        LogUtil.LogError("找不到预设材质 路径为:" + AutoUIConfig.config.FontAssets.Title.MaterialPreset.Shadow.Path);
                        return;
                    }
                    tmp.fontSharedMaterial = presetMaterial;
                }
            }



            /////// 刷新一次tmp组件
            tmp.ForceMeshUpdate();  // <-- 这个非常重要
            AssetDatabase.SaveAssets();
        }
        public static float CorrectSizeValue(float value)
        {
            if (AutoUIConfig.config.Default.Font.EnableCorrect)
            {
                value *= AutoUIConfig.config.Default.Font.CorrectValue;
            }
            return value;
        }
    }

}