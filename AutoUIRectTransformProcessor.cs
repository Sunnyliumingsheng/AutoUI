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
        // todo: 在不修改位置样式的情况下，修改rectTransform的anchor的值
        public static void UpdateRectTransformProcessor(ref GameObject gameObject, ERectTransformMode eRectTransformMode)
        {
            UnityEngine.RectTransform rectTransform = gameObject.GetComponent<UnityEngine.RectTransform>();
            if (rectTransform == null)
            {
                Debug.LogWarning("GameObject does not have RectTransform.");
                return;
            }

            // Step 1: 记录当前 RectTransform 的世界空间矩形信息
            Vector3[] worldCorners = new Vector3[4];
            rectTransform.GetWorldCorners(worldCorners); // 0:左下，1:左上，2:右上，3:右下
            Vector3 bottomLeft = worldCorners[0];
            Vector3 topRight = worldCorners[2];

            // Step 2: 获取当前父物体
            UnityEngine.RectTransform parent = rectTransform.parent as UnityEngine.RectTransform;
            if (parent == null)
            {
                Debug.LogWarning("RectTransform has no parent RectTransform.");
                return;
            }

            // Step 3: 根据目标 eRectTransformMode 设置 anchorMin / anchorMax / pivot
            Vector2 newAnchorMin, newAnchorMax, newPivot;


            GetAnchorAndPivotFromMode(eRectTransformMode, out newAnchorMin, out newAnchorMax, out newPivot);

            // Step 4: 在改变 anchor 前，记录本地坐标下的四个角
            Vector2 localBottomLeft;
            Vector2 localTopRight;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, RectTransformUtility.WorldToScreenPoint(null, bottomLeft), null, out localBottomLeft);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, RectTransformUtility.WorldToScreenPoint(null, topRight), null, out localTopRight);

            // Step 5: 设置 anchor 和 pivot
            rectTransform.anchorMin = newAnchorMin;
            rectTransform.anchorMax = newAnchorMax;
            rectTransform.pivot = newPivot;

            // Step 6: 设置 anchoredPosition 和 sizeDelta 来恢复外观
            Vector2 newSize = localTopRight - localBottomLeft;
            rectTransform.sizeDelta = newSize;

            // 计算中心点
            Vector2 center = (localBottomLeft + localTopRight) / 2f;
            rectTransform.anchoredPosition = center;
        }
        public static void GetAnchorAndPivotFromMode(ERectTransformMode mode, out Vector2 anchorMin, out Vector2 anchorMax, out Vector2 pivot)
        {
            switch (mode)
            {
                case ERectTransformMode.middleCenter:
                    anchorMin = anchorMax = new Vector2(0.5f, 0.5f);
                    pivot = new Vector2(0.5f, 0.5f);
                    break;
                case ERectTransformMode.stretchStretch:
                    anchorMin = new Vector2(0f, 0f);
                    anchorMax = new Vector2(1f, 1f);
                    pivot = new Vector2(0.5f, 0.5f);
                    break;

                // 左侧
                case ERectTransformMode.leftTop:
                    anchorMin = anchorMax = new Vector2(0f, 1f);
                    pivot = new Vector2(0f, 1f);
                    break;
                case ERectTransformMode.leftCenter:
                    anchorMin = anchorMax = new Vector2(0f, 0.5f);
                    pivot = new Vector2(0f, 0.5f);
                    break;
                case ERectTransformMode.leftBottom:
                    anchorMin = anchorMax = new Vector2(0f, 0f);
                    pivot = new Vector2(0f, 0f);
                    break;
                case ERectTransformMode.leftStretch:
                    anchorMin = new Vector2(0f, 0f);
                    anchorMax = new Vector2(0f, 1f);
                    pivot = new Vector2(0f, 0.5f);
                    break;

                // 中间
                case ERectTransformMode.middleTop:
                    anchorMin = anchorMax = new Vector2(0.5f, 1f);
                    pivot = new Vector2(0.5f, 1f);
                    break;
                case ERectTransformMode.middleBottom:
                    anchorMin = anchorMax = new Vector2(0.5f, 0f);
                    pivot = new Vector2(0.5f, 0f);
                    break;
                case ERectTransformMode.middleStretch:
                    anchorMin = new Vector2(0.5f, 0f);
                    anchorMax = new Vector2(0.5f, 1f);
                    pivot = new Vector2(0.5f, 0.5f);
                    break;

                // 右侧
                case ERectTransformMode.rightTop:
                    anchorMin = anchorMax = new Vector2(1f, 1f);
                    pivot = new Vector2(1f, 1f);
                    break;
                case ERectTransformMode.rightCenter:
                    anchorMin = anchorMax = new Vector2(1f, 0.5f);
                    pivot = new Vector2(1f, 0.5f);
                    break;
                case ERectTransformMode.rightBottom:
                    anchorMin = anchorMax = new Vector2(1f, 0f);
                    pivot = new Vector2(1f, 0f);
                    break;
                case ERectTransformMode.rightStretch:
                    anchorMin = new Vector2(1f, 0f);
                    anchorMax = new Vector2(1f, 1f);
                    pivot = new Vector2(1f, 0.5f);
                    break;

                // 横向 Stretch
                case ERectTransformMode.StretchTop:
                    anchorMin = new Vector2(0f, 1f);
                    anchorMax = new Vector2(1f, 1f);
                    pivot = new Vector2(0.5f, 1f);
                    break;
                case ERectTransformMode.StretchCenter:
                    anchorMin = new Vector2(0f, 0.5f);
                    anchorMax = new Vector2(1f, 0.5f);
                    pivot = new Vector2(0.5f, 0.5f);
                    break;
                case ERectTransformMode.StretchBottom:
                    anchorMin = new Vector2(0f, 0f);
                    anchorMax = new Vector2(1f, 0f);
                    pivot = new Vector2(0.5f, 0f);
                    break;

                default:
                    LogUtil.LogWarning($"ERectTransformMode '{mode}' not recognized. Using middleCenter as fallback.");
                    anchorMin = anchorMax = new Vector2(0.5f, 0.5f);
                    pivot = new Vector2(0.5f, 0.5f);
                    break;
            }
        }

        // 第一个数，0代表left，1代表middle，2代表right,3代表stretch
        // 第二个数，0代表bottom，1代表center，2代表top,3代表stretch
        // 简单来说，从左到右，从下到上
        public static ERectTransformMode NumberGetRectTransformMode(int a, int b)
        {
            if (a < 0 || a > 3 || b < 0 || b > 3)
            {
                LogUtil.LogWarning("输入的数字超出范围");
                return ERectTransformMode.middleCenter;
            }
            switch (a)
            {
                case 0: // left
                    switch (b)
                    {
                        case 0: return ERectTransformMode.leftBottom;
                        case 1: return ERectTransformMode.leftCenter;
                        case 2: return ERectTransformMode.leftTop;
                        case 3: return ERectTransformMode.leftStretch;
                    }
                    break;

                case 1: // middle
                    switch (b)
                    {
                        case 0: return ERectTransformMode.middleBottom;
                        case 1: return ERectTransformMode.middleCenter;
                        case 2: return ERectTransformMode.middleTop;
                        case 3: return ERectTransformMode.middleStretch;
                    }
                    break;

                case 2: // right
                    switch (b)
                    {
                        case 0: return ERectTransformMode.rightBottom;
                        case 1: return ERectTransformMode.rightCenter;
                        case 2: return ERectTransformMode.rightTop;
                        case 3: return ERectTransformMode.rightStretch;
                    }
                    break;

                case 3: // stretch (横向)
                    switch (b)
                    {
                        case 0: return ERectTransformMode.StretchBottom;
                        case 1: return ERectTransformMode.StretchCenter;
                        case 2: return ERectTransformMode.StretchTop;
                        case 3: return ERectTransformMode.stretchStretch;
                    }
                    break;
            }

            // 理论上永远不会走到这里
            LogUtil.LogWarning("未匹配到任何 RectTransformMode");
            return ERectTransformMode.middleCenter;

        }

    }

}
