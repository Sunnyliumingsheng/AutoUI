

using UnityEngine;
using UnityEngine.UI;

namespace AutoUI
{
    // 这个类是一个对Pixel进行处理的元操作
    public class AutoUIPictureTool
    {
        public static void PictureLayerGameObjectAddSprite(GameObject gameObject, Sprite sprite, Layer layer)
        {
            Image image = gameObject.AddComponent<Image>();
            image.sprite = sprite;
            Color color = image.color;
            color.a = layer.opacity;
            image.color = color;
            if (sprite.border != Vector4.zero)
            {
                // 这是九宫格
                image.type = Image.Type.Sliced;
            }
        }
        public static void 添加图片sprite(ref GameObject gameobject, in Layer layer)
        {
            FindSpriteResult result = AutoUIAssets.GetSprite(layer.name);
            if (result == null)
            {
                LogUtil.LogError("无法找到对应的sprite:" + layer.name);
                return;
            }
            switch (result.status)
            {
                case EFindAssetStatus.oneResult:
                    Sprite sprite = result.oneResult.sprite;
                    PictureLayerGameObjectAddSprite(gameobject, sprite, layer);
                    break;
                case EFindAssetStatus.manyResult:
                    LogUtil.LogWarning("出现了多个同名的sprite:" + layer.name + "需要手动解决");
                    break;
                case EFindAssetStatus.cantFind:
                    LogUtil.LogWarning("没有找到对应的sprite:" + layer.name);
                    break;
                default:
                    LogUtil.LogError("出现了无法解析的EFIndAssetStatus:" + result.status);
                    break;
            }
        }

    }
}