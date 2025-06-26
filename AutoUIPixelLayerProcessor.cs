using UnityEngine;

namespace AutoUI
{
    public class AutoUIPixelLayerProcessor
    {
        public static void PixelLayerProcessor(in Layer layer, ref GameObject pixelGameObject)
        {
            AutoUIPictureTool.添加图片sprite(ref pixelGameObject, in layer);
        }
    }

}