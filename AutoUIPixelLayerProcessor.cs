using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class AutoUIPixelLayerProcessor
    {
        public  static void PixelLayerProcessor(in Layer layer, ref GameObject pixelGameObject)
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
                    AutoUIPixelTool.PixelLayerGameObjectAddSprite(pixelGameObject, sprite);
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