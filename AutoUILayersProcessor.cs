
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class AutoUILayersProcessor
    {
        public static void LayerProcessor(in Layer layer, ref GameObject layerGameObject)
        {
            switch (layer.eLayerKind)
            {
                case ELayerKind.smartObject:
                    SmartObjectLayerProcessor(in layer);
                    break;
            }
        }


        public static void SmartObjectLayerProcessor(in Layer layer)
        {

        }

        public static void PixelLayerProcessor(in Layer layer)
        {

        }
        public static void TextLayerProcessor(in Layer layer)
        {

        }



    }
}