

using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    // 这个类是一个对Pixel进行处理的元操作
    public class AutoUIPixelTool
    {
        public static void PixelLayerGameObjectAddSprite(GameObject gameObject, Sprite sprite)
        {
            Image image = gameObject.AddComponent<Image>();
            image.sprite = sprite;
            if (sprite.border != Vector4.zero)
            {
                // 这是九宫格
                image.type = Image.Type.Sliced;
            }
        }
    }
}