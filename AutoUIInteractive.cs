using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    // 编辑器窗口类，用来处理交互，是一个巨大的状态机
    public class AutoUIInteractive : EditorWindow
    {
        // 实际上这个方式性能最好，而且还很可控，比如，你想始终将确认按钮放在下面，丢到最下面就好。不想要了直接destory.不必繁琐的逻辑，只要记得这件事，但是总共就这么点，而且逻辑在类里面。
        public static void Interactice()
        {
            AutoUIBoard.line0?.Content();
            DrawLine();
            AutoUIBoard.line1?.Content();
            DrawLine();
            AutoUIBoard.line2?.Content();
            DrawLine();
            AutoUIBoard.line3?.Content();
            DrawLine();
            AutoUIBoard.line4?.Content();
        }

        private static void DrawLine()
        {
            EditorGUILayout.Space();
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, Color.gray);
            EditorGUILayout.Space();
        }
    }

    // 保存线内容的类
    public class AutoUIBoard
    {
        public static ProductBase line0;
        public static ProductBase line1;
        public static ProductBase line2;
        public static ProductBase line3;
        public static ProductBase line4;
    }

    // 产品的抽象基类，提供默认实现
    public abstract class ProductBase
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
            ProductBase.isShow = true;
        }

        public void Hide()
        {
            ProductBase.isShow = false;
        }

        public void Destroy()
        {
            switch (ProductBase.lineOffset)
            {
                case 0: AutoUIBoard.line0 = null; break;
                case 1: AutoUIBoard.line1 = null; break;
                case 2: AutoUIBoard.line2 = null; break;
                case 3: AutoUIBoard.line3 = null; break;
                case 4: AutoUIBoard.line4 = null; break;
            }
        }

        public bool IsShow()
        {
            return ProductBase.isShow;
        }

        public abstract void Content();
        public abstract ProductBase CreateProduct();
    }

    // 用于创建产品的管理类
    public class GUIManager
    {
        public static ProductBase CreateGUINotSelectSprite()
        {
            return new GUINotSelectSprite();
        }
    }

    // 产品类，继承ProductBase，管理事件
    public class GUINotSelectSprite : ProductBase
    {
        string srcPath;

        public override ProductBase CreateProduct()
        {
            var product = new GUINotSelectSprite();
            return product;
        }

        public override void Content()
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
