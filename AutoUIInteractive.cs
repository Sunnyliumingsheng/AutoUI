using UnityEngine;
using UnityEditor;
using System;
namespace Assets.Scripts.Tools.Editor.AutoUI
{

    // 这个类用来处理交互,是一个巨大的状态机
    public class AutoUIInteractive : EditorWindow
    {
        //实际上这个方式性能最好，而且还很可控，比如，你想始终将确认按钮放在下面，丢到最下面就好。不想要了直接destory.不必繁琐的逻辑，只要记得这件事，但是总共就这么点，而且逻辑在类里面。
        public static void Interactice()
        {
            AutoUIBoard.line0.Content();
            DrawLine();
            AutoUIBoard.line1.Content();
            DrawLine();
            AutoUIBoard.line2.Content();
            DrawLine();
            AutoUIBoard.line3.Content();
            DrawLine();
            AutoUIBoard.line4.Content();
        }
        private static void DrawLine()
        {
            EditorGUILayout.Space();
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, Color.gray);
            EditorGUILayout.Space();
        }
    }

    public class AutoUIBoard
    {
        public static IProduct line0;
        public static IProduct line1;
        public static IProduct line2;
        public static IProduct line3;
        public static IProduct line4;
    }

    public interface IProduct
    {
        protected static bool isShow;
        public static int lineOffset = -1;
        public void PutOnLine1()
        {
            AutoUIBoard.line1 = this;
            lineOffset = 1;
        }
        public void PutOnLine2()
        {
            AutoUIBoard.line2 = this;
            lineOffset = 2;
        }
        public void PutOnLine3()
        {
            AutoUIBoard.line3 = this;
            lineOffset = 3;
        }
        public void PutOnLine4()
        {
            AutoUIBoard.line4 = this;
            lineOffset = 4;
        }
        public void PutOnLine0()
        {
            AutoUIBoard.line0 = this;
            lineOffset = 0;
        }
        public void Show()
        {
            IProduct.isShow = true;
            return;
        }
        public void Hide()
        {
            IProduct.isShow = false;
            return;
        }
        public void Destory()
        {
            switch (IProduct.lineOffset)
            {
                case 0: AutoUIBoard.line0 = null; break;
                case 1: AutoUIBoard.line1 = null; break;
                case 2: AutoUIBoard.line2 = null; break;
                case 3: AutoUIBoard.line3 = null; break;
                case 4: AutoUIBoard.line4 = null; break;
            }
        }
        public bool IsShow() { return IProduct.isShow; }
        public void Content();
        public IProduct CreateProduct();
    }

    public class GUIManager{
        public static IProduct CreateGUINotSelectSprite(){
            return new GUINotSelectSprite();
        }
    }

    // 每个产品都有分别管理着一个事件。
    public class GUINotSelectSprite : IProduct
    {
        string srcPath;
        public IProduct CreateProduct()
        {
            var product = new GUINotSelectSprite();
            return product;
        }
        public void Content()
        {
            EditorGUILayout.LabelField("请选择一个正确的路径用来导入Sprite或者跳过这一步骤");
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("跳过导入图片"))
            {
                AutoUIEventManager.GUINotSelectSpriteEvent.Publish(this, new GUINotSelectSpriteEventArgs(true, "", ""));
            }
            if (GUILayout.Button("选择本地路径导入图片"))
            {
                srcPath = AutoUIFile.GUIChooseImagePath();
                if (srcPath == "")
                {
                    LogUtil.AddWarning("没有成功选择图片");
                    AutoUIEventManager.GUINotSelectSpriteEvent.Publish(this, new GUINotSelectSpriteEventArgs(true, "", ""));
                    return;
                }
                if (srcPath != null)
                {
                    if (GUILayout.Button("选择导入到项目中的路径"))
                    {
                        string selectedFolder = EditorUtility.OpenFolderPanel("选择保存目录", "Assets", "");
                    }
                    EditorGUILayout.LabelField("输入导入图片名称");
                }
                EditorGUILayout.EndVertical();
            }

        }
    }
}