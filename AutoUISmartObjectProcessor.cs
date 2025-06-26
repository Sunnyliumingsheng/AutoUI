using UnityEngine;

namespace AutoUI
{
    public class AutoUISmartObjectLayerProcessor
    {
        public static void SmartObjectLayerProcessor(in Layer layer, ref GameObject smartObjectGameObject)
        {
            AutoUIPictureTool.添加图片sprite(ref smartObjectGameObject,in layer);
        }
            
    }
}