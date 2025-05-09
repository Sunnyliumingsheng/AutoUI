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

        // 模拟主程序



        GetWindow<JsonToUIPrefabCreator>("testAutoUI 面板");
        int i = 0;
        // while (true)
        // {
        //     Debug.Log(i++);
        //     if (i==10000)
        //     {
        //         Debug.Log("时间结束");
        //         return ;
        //     }
        //     if (toggleOption==true)
        //     {
        //         Debug.Log("按下按钮结束");
        //         return;
        //     }
        // }


        Debug.Log("已经结束了");
        ;

    }
    private static string inputText = "默认文本";
    private static float numberValue = 0f;
    private static bool toggleOption = false;
    private static int popupIndex = 0;
    private static string[] popupOptions = { "Middle Center", "Top Left", "Bottom Right" };

    public enum ProcessState
    {
        Run,             // 初始空闲
        WaitingUserInput, // 显示 GUI 等待
    }
    public static ProcessState _state = ProcessState.Run;
    public void Update()
    {
        switch (_state)
        {
            case ProcessState.Run:
                // 初始化开始，切换到等待
                break;

            case ProcessState.WaitingUserInput:
                // 什么都不做，主线程静默
                Debug.Log("等待中");
                break;
        }
    }


    public void OnGUI()
    {
        if (GUILayout.Button("点击暂停/开始"))
        {
            _state = (_state == ProcessState.Run) ? ProcessState.WaitingUserInput : ProcessState.Run;
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
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("当前状态：");
        EditorGUILayout.LabelField("文本：", inputText);
        EditorGUILayout.LabelField("滑动值：", numberValue.ToString("F2"));
        EditorGUILayout.LabelField("选项启用：", toggleOption ? "是" : "否");
        EditorGUILayout.LabelField("锚点：", popupOptions[popupIndex]);
    }



}

#endif