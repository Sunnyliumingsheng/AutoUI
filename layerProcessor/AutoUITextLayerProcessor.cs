
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityFramework;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class AutoUITextLayerProcessor
    {
        public static void TextLayerProcessor(Layer layer, GameObject layerGameObject)
        {
            AutoUI.MainThread.Run(() =>
            {
                // 第一部分，添加TMP
                TextMeshProUGUI tmp = layerGameObject.AddComponent<TextMeshProUGUI>();
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
                var localizationTextTMP = layerGameObject.AddComponent<LocalizationText_TMP>();
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

            });

        }
    }
}