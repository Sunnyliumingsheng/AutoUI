using UnityEditor;
using UnityEngine;
using AutoUI;
public class CustomContextMenu
{
    [MenuItem("GameObject/打印rectTransform信息", false, 0)]
    private static void PrintSelectedRect()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            UnityEngine.RectTransform rectTransform = obj.GetComponent<UnityEngine.RectTransform>();
            LogUtil.ClearLogFile();
            if (rectTransform != null)
            {
                LogUtil.Log("rectTransform信息：");
                LogUtil.Log("AnchorMin: " + rectTransform.anchorMin);
                LogUtil.Log("AnchorMax: " + rectTransform.anchorMax);
                LogUtil.Log("Pivot: " + rectTransform.pivot);
                LogUtil.Log("SizeDelta: " + rectTransform.sizeDelta);
                LogUtil.Log("LocalPosition: " + rectTransform.localPosition);
                LogUtil.Log("offsetMax" + rectTransform.offsetMax);
                LogUtil.Log("offsetMin" + rectTransform.offsetMin);
                LogUtil.Log("Rect" + rectTransform.rect);
                LogUtil.Log("anchoredPosition"+rectTransform.anchoredPosition);
            }

        }
    }

}