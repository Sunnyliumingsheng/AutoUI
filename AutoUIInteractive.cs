using UnityEngine;
using UnityEditor;
using BZTA;

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
    public class GUINotSelectSprite : ProductBase
    {
        string srcPath = "";
        string destPath = "";

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

            if (srcPath == "")
            {
                if (GUILayout.Button("选择本地路径导入图片"))
                {
                    srcPath = AutoUIFile.GUIChooseImagePath();
                    if (srcPath == "")
                    {
                        LogUtil.AddWarning("没有成功选择图片");
                        AutoUIEventManager.GUINotSelectSpriteEvent.Publish(this, new GUINotSelectSpriteEventArgs(true, "", ""));
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
                        AutoUIEventManager.GUINotSelectSpriteEvent.Publish(this, new GUINotSelectSpriteEventArgs(true, "", ""));
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
                    AutoUIEventManager.GUINotSelectSpriteEvent.Publish(this, new GUINotSelectSpriteEventArgs(false, srcPath, destPath));
                }
            }

            EditorGUILayout.EndVertical();
        }
    }
    // 普通图层可以选择这个
    public class GUILayerConfirm : ProductBase
    {
        public override ProductBase CreateProduct()
        {
            return new GUILayerConfirm();
        }
        public override void Content()
        {
            EditorGUILayout.BeginVertical();
            if (GUILayout.Button("确认"))
            {
                // 发布事件
                AutoUIEventManager.GUIConfirmEvent.Publish(this, new GUIConfirmArgs());
            }

            EditorGUILayout.EndVertical();
        }
    }
    public class GUIGroupConfirm : ProductBase
    {
        public override ProductBase CreateProduct()
        {
            return new GUIGroupConfirm();
        }
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
    public class GUIRectTransformMode : ProductBase
    {
        public override ProductBase CreateProduct()
        {
            return new GUIRectTransformMode();
        }
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

    public class GUIOneCertainSprite : ProductBase
    {
        public override ProductBase CreateProduct()
        {
            return new GUIOneCertainSprite();
        }
        public override void Content()
        {
            EditorGUILayout.BeginVertical();
            // 只有一张图片，是确定的。
            EditorGUILayout.LabelField("检索到确定的唯一图片路径为：" + spritePath);
            if (spritePath != "")
            {
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                AutoUIUtil.GUIShowSprite(sprite);
                AutoUIEventManager.GUIOneCertainSpriteEvent.Publish(this, new GUIOneCertainSpriteArgs(sprite));
            }
            EditorGUILayout.EndVertical();
        }
        public void SetSpritePath(string spritePath)
        {
            this.spritePath = spritePath;
        }
        private string spritePath = "";
    }
    public class GUIManySpriteCandidate : ProductBase
    {
        public string[] spritePaths = null;
        public int ChooseSpriteoffset = -1;
        public override ProductBase CreateProduct()
        {
            return new GUIManySpriteCandidate();
        }
        public override void Content()
        {
            EditorGUILayout.BeginHorizontal();
            // 有很多图片，是候选的。
            if (spritePaths != null)
            {
                for (int i = 0; i < spritePaths.Length; i++)
                {
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePaths[i]);
                    AutoUIUtil.GUIShowSprite(sprite);
                    if (ChooseSpriteoffset != i)
                    {
                        if (GUILayout.Button("选择这个Sprite"))
                        {
                            // 选择这个Sprite
                            // 发布事件
                            ChooseSpriteoffset = i;
                            AutoUIEventManager.GUIManySpriteCandidateEvent.Publish(this, new GUIManySpriteCandidateArgs(spritePaths[i]));
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
        public void SetSpritePath(string[] spritePaths)
        {
            this.spritePaths = spritePaths;
        }
    }
























}
