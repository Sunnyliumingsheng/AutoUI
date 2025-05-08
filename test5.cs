#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Tuyoo.Render;
using Assets.Scripts.Tools.Editor.AutoUI;


public class JsonToUIPrefabCreator : EditorWindow
{
    static string UIPath = null;
    public static string searchString = "SP_Cmn_Btn_jiasutubiao_MoRen";
    //  核心逻辑代码
    [MenuItem("Tools/TestAutoUI")]
    public static void CreateUIPrefabFromJson()
    {
        GetWindow<JsonToUIPrefabCreator>("testAutoUI 面板");

        try
        {
            string[] result = AssetDatabase.FindAssets($"{searchString} t:Sprite");
            foreach (string guid in result)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                LogUtil.Log($"找到Sprite路径: {path}");
            }
            UIPath = AssetDatabase.GUIDToAssetPath(result[0]);
        }
        catch (Exception e)
        {
            LogUtil.LogError($"生成UI预制体时出错: {e.Message}");
        }
    }
    private static string inputText = "默认文本";
    private static float numberValue = 0f;
    private static bool toggleOption = false;
    private static int popupIndex = 0;
    private static string[] popupOptions = { "Middle Center", "Top Left", "Bottom Right" };

    public void OnGUI()
    {
        if (UIPath != null)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(UIPath);

            Texture2D preview = AssetPreview.GetAssetPreview(sprite);
            if (preview != null)
            {
                GUILayout.Label(preview, GUILayout.Width(64), GUILayout.Height(64));
                GUILayout.Label("图片路径: " + UIPath);
                GUILayout.Button("确认是这个");
            }
            else
            {
                GUILayout.Label("正在生成预览...");
                Repaint();  // 让窗口刷新直到预览生成
            }
        }
        GUILayout.Label("自定义 UI 编辑器", EditorStyles.boldLabel);

        // 1. 文本输入框
        inputText = EditorGUILayout.TextField("输入文字:", inputText);

        // 2. 数值滑动条 (float)
        numberValue = EditorGUILayout.Slider("大小比例:", numberValue, 0f, 100f);

        // 3. 复选框
        toggleOption = EditorGUILayout.Toggle("是否启用选项:", toggleOption);

        // 4. 下拉选择框
        popupIndex = EditorGUILayout.Popup("锚点选项:", popupIndex, popupOptions);

        // 5. 按钮
        if (GUILayout.Button("确认"))
        {

        }

        // 6. 实时刷新显示当前状态
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("当前状态：");
        EditorGUILayout.LabelField("文本：", inputText);
        EditorGUILayout.LabelField("滑动值：", numberValue.ToString("F2"));
        EditorGUILayout.LabelField("选项启用：", toggleOption ? "是" : "否");
        EditorGUILayout.LabelField("锚点：", popupOptions[popupIndex]);
    }



}

#endif