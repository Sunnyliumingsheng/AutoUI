using TMPro;
using UnityEditor;
using UnityEngine;
using UnityFramework;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class AutoUITextLayerProcessor
    {
        public static void TextLayerProcessor(in Layer layer, ref GameObject textGameObject)
        {
            ///////// 基础设置
            TextMeshProUGUI tmp = textGameObject.AddComponent<TextMeshProUGUI>();

            // 文本
            tmp.text = layer.textLayerData.text;

            // 字体大小
            tmp.fontSize = Mathf.RoundToInt(layer.textLayerData.fontSize);

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
                Material presetMaterial = AssetDatabase.LoadAssetAtPath<Material>(AutoUIConfig.config.MaterialPresets.Shadow.Path);
                if (presetMaterial == null)
                {
                    LogUtil.LogError("找不到预设材质 路径为:" + AutoUIConfig.config.MaterialPresets.Shadow.Path);
                    return;
                }
                LogUtil.Log(presetMaterial.name);
                tmp.fontSharedMaterial = presetMaterial;
            }

            // 是否换行
            tmp.enableWordWrapping = false;


            /////// 添加组件


            // 本地化组件
            var localizationTextTMP = textGameObject.AddComponent<LocalizationText_TMP>();
            var field = typeof(LocalizationText_TMP).GetField("mLabel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(localizationTextTMP, tmp);
            EditorUtility.SetDirty(localizationTextTMP);
            

        }
    }
}