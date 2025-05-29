using UnityEngine;


namespace Assets.Scripts.Tools.Editor.AutoUI
{


    class AutoUIRectTransformProcessor
    {

        public static void RectTransformProcessor(ref UnityEngine.RectTransform rectTransform, in RectTransform layerRectTransformData)
        {
            rectTransform.anchoredPosition = layerRectTransformData.anchoredPosition.ToVector2();
            rectTransform.sizeDelta = layerRectTransformData.sizeDelta.ToVector2();
            rectTransform.anchorMin = layerRectTransformData.anchor[0].ToVector2();
            rectTransform.anchorMax = layerRectTransformData.anchor[1].ToVector2();
            rectTransform.pivot = layerRectTransformData.pivot.ToVector2();
        }
    }

}
