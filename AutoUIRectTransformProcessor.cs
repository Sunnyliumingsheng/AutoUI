using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{


    class AutoUIRectTransformProcessor
    {

        public static void RectTransformProcessor(ref UnityEngine.RectTransform rectTransform, in RectTransform layerRectTransformData)
        {
            IUICoordinateSystem iUICoordinateSystem = layerRectTransformData.GetIUICoordinateSystem();
            ERectTransformMode eRectTransformMode = iUICoordinateSystem.GetMode();
            // 设置一下锚点
            rectTransform.anchorMin = new Vector2(layerRectTransformData.anchor[0].x, layerRectTransformData.anchor[0].y);
            rectTransform.anchorMax = new Vector2(layerRectTransformData.anchor[1].x, layerRectTransformData.anchor[1].y);
            // 设置一下pivot
            rectTransform.pivot = new Vector2(layerRectTransformData.pivot.x, layerRectTransformData.pivot.y);
            switch (eRectTransformMode)
            {
                case ERectTransformMode.middleCenter:
                    rectTransform.anchoredPosition = new Vector2(layerRectTransformData.middleCenterModeData.posX, layerRectTransformData.middleCenterModeData.posY);
                    rectTransform.sizeDelta = new Vector2(layerRectTransformData.middleCenterModeData.width, layerRectTransformData.middleCenterModeData.height);
                    break;
                case ERectTransformMode.stretchStretch:
                    rectTransform.offsetMin = new Vector2(layerRectTransformData.stretchStretchModeData.left, layerRectTransformData.stretchStretchModeData.bottom);
                    rectTransform.offsetMax = new Vector2(-layerRectTransformData.stretchStretchModeData.right, -layerRectTransformData.stretchStretchModeData.top);
                    break;
            }
        }


    }

}
