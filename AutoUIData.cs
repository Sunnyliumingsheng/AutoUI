using System.Collections.Generic;
using System.Text;


using Newtonsoft.Json;
using Tuyoo.Render;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class MyComponent
    {
        public string name;
        public Dictionary<string, object> properties;
    }

    // 有以下几种层级，特殊的是group 有layers,但是没有layerData  . canvas可能没有rectTransform
    public enum ELayerKind
    {
        group,
        smartObject,
        pixel,
        text,
        canvas,
    }
    public enum ERectTransformMode
    {
        middleCenter,
        stretchStretch,
    }


    [System.Serializable]
    public class NormalizedPoint
    {
        public float x;
        public float y;
    }
    [System.Serializable]
    public class ColorRGB
    {
        public float r;
        public float g;
        public float b;
    }
    [System.Serializable]
    public class ColorRGBA
    {
        public float r;
        public float g;
        public float b;
        public float a;
    }

    [System.Serializable]
    public class Layer
    {
        public RectTransform rectTransform;
        public string name;
        public bool visible;
        public string layerKind;
        // 这个ELayerKind在json中并不存在
        public ELayerKind eLayerKind;
        public CanvasLayer canvasLayerData;
        public PixelLayer pixelLayerData;
        public TextLayer textLayerData;
        public SmartObjectLayer smartObjectLayerData;
        public List<Layer> layers;
        public List<MyComponent> components;

        public void VerifyLayers()
        {
            LogUtil.Log("=== 开始进行验证 ===");
            if (this.eLayerKind == ELayerKind.group || this.eLayerKind == ELayerKind.canvas)
            {
                recusionLayers(this.layers);
            }
        }
        public void recusionLayers(List<Layer> layers)
        {
            foreach (var layer in layers)
            {
                LogUtil.Log("层级名" + layer.name + "层级种类" + layer.eLayerKind);
                if (layer.eLayerKind == ELayerKind.group)
                {
                    LogUtil.Log((layer.layers == null).ToString());
                    recusionLayers(layer.layers);
                }
                else
                {
                    switch (layer.eLayerKind)
                    {
                        case ELayerKind.group:
                            var err = new AutoUIException("出现错误");
                            LogUtil.LogError(err);
                            break;
                        default:
                            if (layer.smartObjectLayerData == null && layer.pixelLayerData == null && layer.textLayerData == null && layer.canvasLayerData == null)
                            {
                                var err0 = new AutoUIException("解析失败,这里提供layer的name方便检索" + layer.name);
                                LogUtil.LogError(err0);
                            }
                            break;


                    }
                    LogUtil.Log("层级名" + layer.name + "层级种类" + layer.eLayerKind);
                }
            }
        }
        public ILayerData GetILayerData(){
            switch (this.eLayerKind){
                case ELayerKind.canvas:
                    return this.canvasLayerData;
                case ELayerKind.pixel:
                    return this.pixelLayerData;
                case ELayerKind.text:
                    return this.textLayerData;
                case ELayerKind.smartObject:
                    return this.smartObjectLayerData;
                default:
                    var err = new AutoUIException(" 试图获取ILayerData接口但是遇到了无法解析的elayerKind:"+this.eLayerKind);
                    LogUtil.LogError(err);
                    return null;
            }
        }

    }
    [System.Serializable]
    public class RectTransform
    {
        public string type;
        public NormalizedPoint[] anchor = new NormalizedPoint[2];
        public NormalizedPoint pivot;
        public MiddleCenterMode middleCenterModeData;
        public StretchStretchMode stretchStretchModeData;
        public IUICoordinateSystem GetIUICoordinateSystem()
        {
            if (this.middleCenterModeData != null)
            {
                return this.middleCenterModeData;
            }
           if (this.stretchStretchModeData!= null){
                return this.stretchStretchModeData;
           }
           return null;
        }
    }


    public interface IUICoordinateSystem
    {
        public ERectTransformMode GetMode();
    }
    [System.Serializable]
    public class StretchStretchMode : IUICoordinateSystem
    {
        public float top;
        public float bottom;
        public float left;
        public float right;
        public ERectTransformMode GetMode()
        {
            return ERectTransformMode.stretchStretch;
        }
    }
    [System.Serializable]
    public class MiddleCenterMode : IUICoordinateSystem
    {
        public float posX;
        public float posY;
        public float width;
        public float height;
        public ERectTransformMode GetMode()
        {
            return ERectTransformMode.middleCenter;
        }
    }
    public interface ILayerData { }

    [System.Serializable]
    public class PixelLayer : ILayerData
    {
        public string kind;
    }

    [System.Serializable]
    public class TextLayer : ILayerData
    {
        public string kind;
        public string text;
        public float fontSize;
        public ColorRGB color;
        public string textAlign;
    }

    [System.Serializable]
    public class SmartObjectLayer : ILayerData
    {
        public float width;
        public float height;
        public string fileReference;
        public string kind;
    }

    [System.Serializable]
    public class CanvasLayer : ILayerData
    {
        public float width;
        public float height;
        public string renderMode;
    }
}