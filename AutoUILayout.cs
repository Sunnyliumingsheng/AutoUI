using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace AutoUI
{
    public class AutoUILayoutProcessor
    {
        /// <summary>
        ///  需要注意的是，这里必须使用解读layer的方式进行推导，否则必须再加一个循环，因为子对象并没有创建好
        ///  绞尽脑汁都想不出一个完美方案。
        ///  整体思路就是不追求完美，主要还是让程序自己去填写，有了个大概即可。否则对美术来说可能会有点复杂
        /// less is more?
        /// </summary>



        /// 警告 可能很不靠谱
        public static void GridLayout参数自动推导(in Layer layer, ref GameObject parentGameObject)
        {
            if (layer == null)
            {
                LogUtil.LogError("layer is null");
                return;
            }

            int column = 0;
            int row = 0;

            // 读取 layout 参数
            foreach (var component in layer.components)
            {
                if (component.name == "grid")
                {
                    component.parameters.TryGetValue("column", out object tryGetColumn);
                    component.parameters.TryGetValue("row", out object tryGetRow);
                    if (tryGetColumn != null) column = Convert.ToInt32(tryGetColumn);
                    if (tryGetRow != null) row = Convert.ToInt32(tryGetRow);
                    break;
                }
            }

            if (column == 0 && row == 0)
            {
                LogUtil.LogWarning($"gridLayout 的 column 和 row 必须至少有一个。Layer: {layer.name}");
                return;
            }

            GridLayoutGroup gridLayout = parentGameObject.GetComponent<GridLayoutGroup>();
            if (gridLayout == null)
            {
                LogUtil.LogError("未找到 GridLayoutGroup 组件");
                return;
            }

            if (layer.eLayerKind != ELayerKind.group)
            {
                LogUtil.LogError("gridLayout 只能用于 group 图层");
                return;
            }

            var childLayers = layer.layers.Where(l => l.eLayerKind == ELayerKind.group).ToList();
            if (childLayers.Count < 2)
            {
                LogUtil.LogWarning($"使用了 gridLayout 但子图层数量不足2个。Layer: {layer.name}");
                return;
            }

            RectTransform first = childLayers[0].rectTransform;
            RectTransform second = childLayers[1].rectTransform;

            Vector2 cellSize = first.sizeDelta.ToVector2();
            float spacingX = 0, spacingY = 0;

            if (column > 1 && row == 0)
            {
                // 横向布局（固定列数）
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayout.constraintCount = column;

                if (column > 1 && first != null && second != null)
                {
                    float firstRight = first.anchoredPosition.x + first.sizeDelta.x * (1 - first.pivot.x);
                    float secondLeft = second.anchoredPosition.x - second.sizeDelta.x * second.pivot.x;
                    spacingX = secondLeft - firstRight;
                }

                spacingY = 0f;
            }
            else if (row > 1 && column == 0)
            {
                // 纵向布局（固定行数）
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                gridLayout.constraintCount = row;

                if (row > 1 && first != null && second != null)
                {
                    float firstBottom = first.anchoredPosition.y - first.sizeDelta.y * first.pivot.y;
                    float secondTop = second.anchoredPosition.y + second.sizeDelta.y * (1 - second.pivot.y);
                    spacingY = firstBottom - secondTop;
                }

                spacingX = 0f;
            }
            else if (column == 1 && row == 0)
            {
                // 仅 1 列，推断为纵向布局
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayout.constraintCount = 1;

                if (first != null && second != null)
                {
                    float firstBottom = first.anchoredPosition.y - first.sizeDelta.y * first.pivot.y;
                    float secondTop = second.anchoredPosition.y + second.sizeDelta.y * (1 - second.pivot.y);
                    spacingY = firstBottom - secondTop;
                }

                spacingX = 0f;
            }
            else if (row == 1 && column == 0)
            {
                // 仅 1 行，推断为横向布局
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                gridLayout.constraintCount = 1;

                if (first != null && second != null)
                {
                    float firstRight = first.anchoredPosition.x + first.sizeDelta.x * (1 - first.pivot.x);
                    float secondLeft = second.anchoredPosition.x - second.sizeDelta.x * second.pivot.x;
                    spacingX = secondLeft - firstRight;
                }

                spacingY = 0f;
            }
            else if (column > 1 && row > 1)
            {
                // 都设置了，优先以列为主（你也可以选择优先行）
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                gridLayout.constraintCount = column;

                if (first != null && second != null)
                {
                    float firstRight = first.anchoredPosition.x + first.sizeDelta.x * (1 - first.pivot.x);
                    float secondLeft = second.anchoredPosition.x - second.sizeDelta.x * second.pivot.x;
                    spacingX = secondLeft - firstRight;
                }

                spacingY = 0f;
            }
            else
            {
                // 未设置 row/column，默认不限制，spacing 为 0
                LogUtil.LogWarning("未设置 row/column默认不限制spacing 为 0"+layer.name);
                gridLayout.constraint = GridLayoutGroup.Constraint.Flexible;
                spacingX = 0f;
                spacingY = 0f;
            }

            // 设置间距
            gridLayout.spacing = new Vector2(spacingX, spacingY);

            // 设置 padding
            float padding = AutoUIConfig.config.Default.Layout.Padding;
            gridLayout.padding = new RectOffset((int)padding, (int)padding, (int)padding, (int)padding);

            // 设置 cell 和 spacing
            gridLayout.cellSize = cellSize;
            gridLayout.spacing = new Vector2(spacingX, spacingY);
        }

        // 警告 很有可能很不靠谱
        public static void ApplyVerticalLayout(in Layer layer, ref GameObject parent)
        {
            if (layer == null || layer.eLayerKind != ELayerKind.group)
            {
                LogUtil.LogError("仅 group 层可以使用 VerticalLayoutGroup");
                return;
            }

            var layout = parent.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = parent.AddComponent<VerticalLayoutGroup>();
            }

            List<Layer> children = layer.layers.Where(l => l.eLayerKind == ELayerKind.group).ToList();
            if (children.Count < 2)
            {
                LogUtil.LogWarning($"图层 '{layer.name}' 子元素过少，跳过布局推导");
                return;
            }

            // 按 Y 倒序排列（Unity UI 是 Y 向下）
            children = children.OrderByDescending(c => c.rectTransform.anchoredPosition.y).ToList();

            RectTransform r1 = children[0].rectTransform;
            RectTransform r2 = children[1].rectTransform;

            float spacing = (r1.anchoredPosition.y - r1.sizeDelta.y) - r2.anchoredPosition.y;
            if (spacing < 0)
                spacing = 0;

            float maxTop = children.Max(c => c.rectTransform.anchoredPosition.y);
            float minBottom = children.Min(c => c.rectTransform.anchoredPosition.y - c.rectTransform.sizeDelta.y);
            float containerHeight = parent.GetComponent<RectTransform>().sizeDelta.y;
            float paddingTop = containerHeight - maxTop;
            float paddingBottom = minBottom;

            layout.spacing = spacing;
            layout.padding = new RectOffset(0, 0, (int)paddingTop, (int)paddingBottom);
            layout.childAlignment = TextAnchor.UpperLeft;

            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }
        public static void ApplyHorizontalLayout(in Layer layer, ref GameObject parent)
        {
            if (layer == null || layer.eLayerKind != ELayerKind.group)
            {
                LogUtil.LogError("仅 group 层可以使用 HorizontalLayoutGroup");
                return;
            }

            var layout = parent.GetComponent<HorizontalLayoutGroup>();
            if (layout == null)
            {
                layout = parent.AddComponent<HorizontalLayoutGroup>();
            }

            List<Layer> children = layer.layers.Where(l => l.eLayerKind == ELayerKind.group).ToList();
            if (children.Count < 2)
            {
                LogUtil.LogWarning($"图层 '{layer.name}' 子元素过少，跳过布局推导");
                return;
            }

            // 推导 spacing
            RectTransform r1 = children[0].rectTransform;
            RectTransform r2 = children[1].rectTransform;

            float spacing = r2.anchoredPosition.x - (r1.anchoredPosition.x + r1.sizeDelta.x);
            if (spacing < 0)
                spacing = 0;

            // 推导 padding
            float minLeft = children.Min(c => c.rectTransform.anchoredPosition.x);
            float maxRight = children.Max(c => c.rectTransform.anchoredPosition.x + c.rectTransform.sizeDelta.x);
            float containerWidth = parent.GetComponent<RectTransform>().sizeDelta.x;
            float paddingLeft = minLeft;
            float paddingRight = containerWidth - maxRight;

            layout.spacing = spacing;
            layout.padding = new RectOffset((int)paddingLeft, (int)paddingRight, 0, 0);
            layout.childAlignment = TextAnchor.UpperLeft;

            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
        }
    }
}