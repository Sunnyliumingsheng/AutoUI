using UnityEngine.UI;
using UnityEditor;
using UnityEngine.EventSystems;
using AutoUI;
using UnityEngine;

public class AutoUIScrollCreator
{
    [MenuItem("Tools/TestAutoUIScroll")]
    public static void Main()
    {
        LogUtil.Log("=== AutoUI start ===");
        AutoUIConfig.GetAutoUIConfigData();

        // 创建根对象
        GameObject root = new GameObject("AutoScrollViewRoot", typeof(UnityEngine.RectTransform));
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();

        // Scroll View 对象
        GameObject scrollViewGO = new GameObject("Scroll View", typeof(UnityEngine.RectTransform), typeof(Image), typeof(ScrollRect));
        scrollViewGO.transform.SetParent(root.transform, false);
        UnityEngine.RectTransform scrollRectTransform = scrollViewGO.GetComponent<UnityEngine.RectTransform>();
        scrollRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        scrollRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        scrollRectTransform.pivot = new Vector2(0.5f, 0.5f);
        scrollRectTransform.sizeDelta = new Vector2(400, 300);
        scrollRectTransform.anchoredPosition = Vector2.zero;

        Image scrollImage = scrollViewGO.GetComponent<Image>();
        scrollImage.color = new Color(1, 1, 1, 0.25f);

        ScrollRect scrollRect = scrollViewGO.GetComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;

        // Viewport
        GameObject viewportGO = new GameObject("Viewport", typeof(UnityEngine.RectTransform), typeof(Image), typeof(Mask));
        viewportGO.transform.SetParent(scrollViewGO.transform, false);
        UnityEngine.RectTransform viewportRT = viewportGO.GetComponent<UnityEngine.RectTransform>();
        viewportRT.anchorMin = Vector2.zero;
        viewportRT.anchorMax = Vector2.one;
        viewportRT.offsetMin = Vector2.zero;
        viewportRT.offsetMax = Vector2.zero;
        viewportGO.GetComponent<Image>().color = new Color(1, 1, 1, 0.1f);
        viewportGO.GetComponent<Mask>().showMaskGraphic = false;

        // Content
        GameObject contentGO = new GameObject("Content", typeof(UnityEngine.RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        contentGO.transform.SetParent(viewportGO.transform, false);
        UnityEngine.RectTransform contentRT = contentGO.GetComponent<UnityEngine.RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1);
        contentRT.anchorMax = new Vector2(1, 1);
        contentRT.pivot = new Vector2(0.5f, 1);
        contentRT.anchoredPosition = Vector2.zero;
        contentRT.sizeDelta = new Vector2(0, 600);

        // LayoutGroup 设置
        VerticalLayoutGroup layoutGroup = contentGO.GetComponent<VerticalLayoutGroup>();
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childControlHeight = true;
        layoutGroup.childControlWidth = true;

        ContentSizeFitter fitter = contentGO.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        // 设置 ScrollRect 引用
        scrollRect.viewport = viewportRT;
        scrollRect.content = contentRT;

        // 创建 EventSystem（如果不存在）
        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        // 保存为预制体
        AutoUIFile.SavePrefabAndCleanup(root);
    }
}
