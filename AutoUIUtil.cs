using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class AutoUIUtil
    {
        public static string scenePath = AutoUIConfig.config.sprite.scenePath;

        public static float PSTextSizeToUnityTMPFontSize(int psTextSize)
        {
            // 这是一个经验系数，请随时调整
            return psTextSize * AutoUIConfig.config.text.fontScale;
        }
        // 打开scene，并进入到预制体中
        public static GameObject OpenPrefabByPath(string prefabPath)
        {
            EditorSceneManager.OpenScene(scenePath);
            var prefab = PrefabStageUtility.OpenPrefab(prefabPath);
            if (prefab != null)
            {
                EditorWindow.FocusWindowIfItsOpen<SceneView>();
                Selection.activeObject = prefab;
            }
            else
            {
                new AutoUIException("打开prefab失败");
            }
            return prefab.prefabContentsRoot;
        }
        public static void GUIShowSprite(Sprite sprite)
        {
            if (sprite != null)
            {
                Texture2D texture = sprite.texture;
                Rect spriteRect = sprite.rect;

                // 计算 UV 坐标
                Rect texCoords = new Rect(
                    spriteRect.x / texture.width,
                    spriteRect.y / texture.height,
                    spriteRect.width / texture.width,
                    spriteRect.height / texture.height
                );

                // 计算缩放比例，适配目标大小
                float scale = Mathf.Min(
                    AutoUIConfig.config.previewSpriteSize.width / spriteRect.width,
                    AutoUIConfig.config.previewSpriteSize.height / spriteRect.height
                );

                float drawWidth = spriteRect.width * scale;
                float drawHeight = spriteRect.height * scale;

                // 居中绘制
                Rect previewRect = GUILayoutUtility.GetRect(AutoUIConfig.config.previewSpriteSize.width, AutoUIConfig.config.previewSpriteSize.height , GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
                Rect centeredRect = new Rect(
                    previewRect.x + (previewRect.width - drawWidth) / 2,
                    previewRect.y + (previewRect.height - drawHeight) / 2,
                    drawWidth,
                    drawHeight
                );

                // 背景框（可选）
                EditorGUI.DrawRect(previewRect, new Color(0.2f, 0.2f, 0.2f, 1f));

                // 绘制 sprite 的 texture 区域
                GUI.DrawTextureWithTexCoords(centeredRect, texture, texCoords, true);
            }
        }
    }
}