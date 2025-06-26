using UnityEngine;


namespace AutoUI
{


    class AutoUIRectTransformProcessor
    {

        public static void RectTransformProcessor(ref UnityEngine.RectTransform rectTransform, in RectTransform layerRectTransformData)
        {
            rectTransform.anchoredPosition = layerRectTransformData.anchoredPosition.ToVector2();
            rectTransform.sizeDelta = layerRectTransformData.sizeDelta.ToVector2();
            if (layerRectTransformData.anchor == null)
            {
                LogUtil.LogError("layerRectTransformData.anchor is null,请检查JSON搜索null,非常非常有可能是美术在图层命名的时候使用到/符号导致");
            }
            rectTransform.anchorMin = layerRectTransformData.anchor[0].ToVector2();
            rectTransform.anchorMax = layerRectTransformData.anchor[1].ToVector2();
            rectTransform.pivot = layerRectTransformData.pivot.ToVector2();
        }
    }

}
