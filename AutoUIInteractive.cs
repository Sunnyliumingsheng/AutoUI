using UnityEngine;
using UnityEditor;
namespace Assets.Scripts.Tools.Editor.AutoUI
{

    // 这个类用来处理交互
    public class AutoUIInteractive : EditorWindow
    {
        private static string inputText = "默认文本";
        private static float numberValue = 0f;
        private static bool toggleOption = false;
        private static int popupIndex = 0;
        private static string[] popupOptions = { "Middle Center", "Top Left", "Bottom Right" };

        public static void Interactice()
        {
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

}