using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class rectTransformUI : UIBase
    {
        private ERectTransformMode nowMode;
        private Action<ERectTransformMode> callback;
        public rectTransformUI(ERectTransformMode originMode,Action<ERectTransformMode> callbackChangeRectTransform)
        {
            nowMode = originMode;
            callback = callbackChangeRectTransform;
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
                    if (mode == nowMode)
                    {
                        GUILayout.TextField("当前" + mode.ToString(), GUILayout.Width(120), GUILayout.Height(30));
                        continue;
                    }
                    if (GUILayout.Button(mode.ToString(), GUILayout.Width(120), GUILayout.Height(30)))
                    {
                        if (callback == null)
                        {
                            LogUtil.LogError("rectTransformUI的callback为null");
                        }
                        callback.Invoke(mode);
                        nowMode = mode;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

    }
}