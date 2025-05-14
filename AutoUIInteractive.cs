using UnityEngine;
using UnityEditor;
using BZTA;
// 提供交互组件，只需要创建完成之后PutOnLine就能展示，记得最后Destroy。设置值的方式是SetValue，返回值的方式是通过时间的回调。
namespace Assets.Scripts.Tools.Editor.AutoUI
{
    // 编辑器窗口类，用来处理交互，是一个巨大的状态机
    public class AutoUIInteractive : EditorWindow
    {
        // 实际上这个方式性能最好，而且还很可控，比如，你想始终将确认按钮放在下面，丢到最下面就好。不想要了直接destory.不必繁琐的逻辑，只要记得这件事，但是总共就这么点，而且逻辑在类里面。
        public static void Interactice()
        {
            if(AutoUIBoard.line0!=null && AutoUIBoard.line0.IsShow()){
                AutoUIBoard.line0.Content();
            }
            DrawLine();
            if(AutoUIBoard.line1!=null && AutoUIBoard.line1.IsShow()){
                AutoUIBoard.line1.Content(); 
            }
            DrawLine();
            if(AutoUIBoard.line2!=null && AutoUIBoard.line2.IsShow()){
                AutoUIBoard.line2.Content();
            }
            DrawLine();
            if(AutoUIBoard.line3!=null && AutoUIBoard.line3.IsShow()){
                AutoUIBoard.line3.Content();
            }
            DrawLine();
            if(AutoUIBoard.line4!=null && AutoUIBoard.line4.IsShow()){
                AutoUIBoard.line4.Content(); 
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
        public static ProductBase line0;
        public static ProductBase line1;
        public static ProductBase line2;
        public static ProductBase line3;
        public static ProductBase line4;
    }

    // 产品的抽象基类，提供默认实现
    public abstract class ProductBase
    {
        protected  bool isShow=true;
        public  int lineOffset = -1;

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
            isShow = true;
        }

        public void Hide()
        {
            isShow = false;
        }

        public void Destroy()
        {
            switch (lineOffset)
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
            return isShow;
        }

        public abstract void Content();
    }

    // 用于创建产品的管理类
    public class GUIManager
    {
        public static ProductBase CreateGUINotSelectSprite()
        {
            return new GUINotFindSprite();
        }
        public static ProductBase CreateGUILayerConfirm()
        {
            return new GUILayerConfirm();
        }
        public static ProductBase CreateGUIRectTransformMode()
        {
            return new GUIRectTransformMode();
        }
        public static ProductBase CreateGUIOneCertainSprite()
        {
            return new GUIOneCertainSprite();
        }
        public static ProductBase CreateGUIManySpriteCandidate()
        {
            return new GUIManySpriteCandidate();
        }
    }

    // 产品类，继承ProductBase，管理事件
    public class GUINotFindSprite : ProductBase
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
                        LogUtil.AddWarning("没有成功选择图片");
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
                        LogUtil.AddWarning("没有成功选择图片");
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
    public class GUILayerConfirm : ProductBase
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
    public class GUIGroupConfirm : ProductBase
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
    public class GUIRectTransformMode : ProductBase
    {
        public override void Content()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Label("选择布局模式", EditorStyles.boldLabel);
            int columns = 4;
            int rows = 4;
            ERectTransformMode[,] modeGrid = new ERectTransformMode[4, 4]
            {
        { ERectTransformMode.leftBottom, ERectTransformMode.middleBottom, ERectTransformMode.rightBottom, ERectTransformMode.StretchBottom },
        { ERectTransformMode.leftCenter, ERectTransformMode.middleCenter, ERectTransformMode.rightCenter, ERectTransformMode.StretchCenter },
        { ERectTransformMode.leftTop, ERectTransformMode.middleTop, ERectTransformMode.rightTop, ERectTransformMode.StretchTop },
        { ERectTransformMode.leftStretch, ERectTransformMode.middleStretch, ERectTransformMode.rightStretch, ERectTransformMode.stretchStretch },
            };

            for (int row = 0; row < rows; row++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int col = 0; col < columns; col++)
                {
                    ERectTransformMode mode = modeGrid[row, col];
                    if (mode == originMode)
                    {
                        GUILayout.TextField("已经选择" + mode.ToString(), GUILayout.Width(120), GUILayout.Height(30));
                        continue;
                    }
                    if (GUILayout.Button(mode.ToString(), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        AutoUIEventManager.GUIChooseNewRectTransformEvent.Publish(this, new GUIChooseNewRectTransformArgs(mode));
                        originMode = mode;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
        private ERectTransformMode originMode = ERectTransformMode.middleCenter;
        public void SetOriginRectTransform(ERectTransformMode eRectTransformMode)
        {
            this.originMode = eRectTransformMode;
        }
    }

    // 像素图层找到了确定的Sprite，可以直接使用，这里只作为展示功能。
    public class GUIOneCertainSprite : ProductBase
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
        private Sprite sprite =null;
    }
    public class GUIManySpriteCandidate : ProductBase
    {
        public Sprite[] sprites=null;
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























}
