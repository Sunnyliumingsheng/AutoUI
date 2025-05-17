using UnityEngine;
using UnityEditor;
using BZTA;
using System.Runtime.CompilerServices;
using FrameworkEditor.BehaviourTree;
// 提供交互组件，只需要创建完成之后PutOnLine就能展示，记得最后Destroy。设置值的方式是SetValue，返回值的方式是通过时间的回调。
namespace Assets.Scripts.Tools.Editor.AutoUI
{
    // 编辑器窗口类，用来处理交互，是一个巨大的状态机
    public class AutoUIInteractive : EditorWindow
    {
        // 实际上这个方式性能最好，而且还很可控，比如，你想始终将确认按钮放在下面，丢到最下面就好。不想要了直接destory.不必繁琐的逻辑，只要记得这件事，但是总共就这么点，而且逻辑在类里面。
        public static void Interactice()
        {
            for (int i = 0; i < AutoUIBoard.lineNum; i++)
            {
                if (AutoUIBoard.IsAlreadyExist(i) && AutoUIBoard.IsLineShow(i))
                {
                    AutoUIBoard.UnsafeShowlineContent(i);
                }
            }
            DrawLine();
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
        public static void AutoUIBoardInit()
        {
            lineNum = AutoUIConfig.config.guiBoard.lineNum;
            line = new UIBase[lineNum];
        }
        public static int lineNum;
        public static UIBase[] line;
        public static bool lineOffset超出范围(int offset)
        {
            return (offset < 0 || offset >= AutoUIBoard.lineNum);
        }
        public static bool IsAlreadyExist(int offset)
        {
            return (line[offset] != null);
        }
        public static void UnsafeShowlineContent(int offset)
        {
            line[offset]?.Content();
        }
        public static void ClearLine(int offset)
        {
            if (!lineOffset超出范围(offset) || IsAlreadyExist(offset))
            {
                line[offset] = null;
            }
        }
        public static bool IsLineShow(int offset)
        {
            if (!lineOffset超出范围(offset) && IsAlreadyExist(offset))
            {
                return line[offset].IsShow();
            }
            return false;
        }
        public static void PutOnLine(UIBase ui, int offset)
        {
            if (offset < 0 || offset >= AutoUIBoard.lineNum)
            {
                LogUtil.LogError("lineOffset超出范围");
                return;
            }
            if (IsAlreadyExist(offset))
            {
                LogUtil.LogError("lineOffset已经存在");
                return;
            }
            line[offset] = ui;
            ui.lineOffset = offset;
        }
        public static void SimplePut(UIBase ui)
        {
            for (int i = 0; i < AutoUIBoard.lineNum; i++)
            {
                if (AutoUIBoard.IsAlreadyExist(i))
                {
                    continue;
                }
                else
                {
                    PutOnLine(ui, i);
                    return;
                }
            }
        }
        public static void PutEndLine(UIBase ui)
        {
            for (int i = AutoUIBoard.lineNum - 1; i >= 0; i--)
            {
                if (lineOffset超出范围(i))
                {
                    LogUtil.LogError("lineOffset超出范围");
                    return;
                }
                if (AutoUIBoard.IsAlreadyExist(i))
                {
                    continue;
                }
                else
                {
                    PutOnLine(ui, i);
                    return;
                }
            }
        }
    }

    // 产品的抽象基类，提供默认实现
    public abstract class UIBase
    {
        protected bool isShow = true;
        public int lineOffset = -1;



        public void Show()
        {
            isShow = true;
        }

        public void Hide()
        {
            isShow = false;
        }

        public void Destroy()
        {
            if (lineOffset == -1)
            {
                LogUtil.LogError("没有找到对应的lineOffset");
            }
            AutoUIBoard.ClearLine(lineOffset);
        }

        public bool IsShow()
        {
            return isShow;
        }

        public abstract void Content();
    }

/*

    // 产品类，继承ProductBase，管理事件
    public class GUINotFindSprite : UIBase
    {
        string srcPath = "";
        string destPath = "";

        public override void Content()
        {
            EditorGUILayout.LabelField("请选择一个正确的路径用来导入Sprite或者跳过这一步骤");
            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("跳过导入图片"))
            {
                AutoUIEventManager.GUINotFindSpriteEvent.Publish(this, new GUINotFindSpriteEventArgs(true, "", ""));
            }

            if (srcPath == "")
            {
                if (GUILayout.Button("选择本地路径导入图片"))
                {
                    srcPath = AutoUIFile.GUIChooseImagePath();
                    if (srcPath == "")
                    {
                        LogUtil.LogWarning("没有成功选择图片");
                        AutoUIEventManager.GUINotFindSpriteEvent.Publish(this, new GUINotFindSpriteEventArgs(true, "", ""));
                        return;
                    }

                }
            }
            else
            {
                EditorGUILayout.LabelField("选择的图片路径为：" + srcPath);
                if (GUILayout.Button("重新选择本地图片路径"))
                {
                    srcPath = AutoUIFile.GUIChooseImagePath();
                    if (srcPath == "")
                    {
                        LogUtil.LogWarning("没有成功选择图片");
                        AutoUIEventManager.GUINotFindSpriteEvent.Publish(this, new GUINotFindSpriteEventArgs(true, "", ""));
                        return;
                    }

                }
            }

            if (destPath == "")
            {
                if (GUILayout.Button("选择导入到项目中的路径"))
                {
                    string destPath = EditorUtility.OpenFolderPanel("选择保存目录", "Assets/GameAssets/ABNew/SpriteAtlas/Sprites", "");
                }
            }
            else
            {
                EditorGUILayout.LabelField("选择的导入路径为：" + destPath);
                if (GUILayout.Button("重新选择导入到项目中的路径"))
                {
                    string destPath = EditorUtility.OpenFolderPanel("选择保存目录", "Assets/GameAssets/ABNew/SpriteAtlas/Sprites", "");
                }
            }

            if (srcPath != "" && destPath != "")
            {
                if (GUILayout.Button("确定导入"))
                {
                    AutoUIEventManager.GUINotFindSpriteEvent.Publish(this, new GUINotFindSpriteEventArgs(false, srcPath, destPath));
                }
            }

            EditorGUILayout.EndVertical();
        }
    }
    // 普通图层可以选择这个
    public class GUILayerConfirm : UIBase
    {
        public override void Content()
        {
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("确认"))
            {
                // 发布事件
                AutoUIEventManager.GUILayerConfirmEvent.Publish(this, new GUILayerConfirmArgs());
            }

            EditorGUILayout.EndVertical();
        }
    }
    // 对于有小组图层，有时候要用到剪枝，但是我现在认为暂时不支持是更明知的选择
    public class GUIGroupConfirm : UIBase
    {
        public override void Content()
        {
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("确认"))
            {

            }
            if (GUILayout.Button("剪枝"))
            {

            }
            EditorGUILayout.EndVertical();
        }
    }
    // 选择布局模式
    */


/*

    // 像素图层找到了确定的Sprite，可以直接使用，这里只作为展示功能。
    public class GUIOneCertainSprite : UIBase
    {
        public override void Content()
        {
            EditorGUILayout.BeginVertical();
            // 只有一张图片，是确定的。
            EditorGUILayout.LabelField("检索到确定的唯一Sprite");
            if (sprite != null)
            {
                AutoUIUtil.GUIShowSprite(sprite);
            }
            EditorGUILayout.EndVertical();
        }
        public void SetSprite(Sprite sprite)
        {
            this.sprite = sprite;
        }
        private Sprite sprite = null;
    }
    public class GUIManySpriteCandidate : UIBase
    {
        public Sprite[] sprites = null;
        public int ChooseSpriteoffset = -1;
        public override void Content()
        {
            EditorGUILayout.BeginHorizontal();
            // 有很多图片，是候选的。
            if (sprites != null)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    AutoUIUtil.GUIShowSprite(sprites[i]);
                    if (ChooseSpriteoffset != i)
                    {
                        if (GUILayout.Button("选择这个Sprite"))
                        {
                            // 选择这个Sprite
                            // 发布事件
                            ChooseSpriteoffset = i;
                            AutoUIEventManager.GUIManySpriteCandidateEvent.Publish(this, new GUIManySpriteCandidateArgs(sprites[i]));
                        }
                    }
                    else
                    {
                        GUILayout.TextField("已选择这个");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        public void SetSprites(Sprite[] sprites)
        {
            this.sprites = sprites;
        }
    }




*/
}
