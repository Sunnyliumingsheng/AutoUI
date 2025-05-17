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
    //  核心逻辑代码
    [MenuItem("Tools/TestAutoUI")]
    public static void CreateUIPrefabFromJson()
    {
        GetWindow<JsonToUIPrefabCreator>("testAutoUI 面板");
        Debug.Log("已经结束了");
    }

    private static int b = 0;
    private static int a = 0;
    public void Update()
    {
        a++;
        if (a == 100000)
        {
            return;
        }
        // 若是没了这个Repaint，就会卡的一批。
        //Repaint();
    }

    public void OnGUI()
    {
        Controllor(this);
        EditorGUILayout.LabelField("输入文本:" + a.ToString() + "///" + b.ToString() + "///");
    }
    public void Controllor(EditorWindow window)
    {
        SonControllor(this);
    }
    public void SonControllor(EditorWindow window)
    {
        for (int i = 0; i < 100000; i++)
        {
            b++;
            Repaint();
        }
    }


}

#endif