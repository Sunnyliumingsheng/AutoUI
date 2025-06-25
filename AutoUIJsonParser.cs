using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public static class LayerJsonParser
    {
        public static Layer ParseFromJson(string json)
        {
            Layer layer = JsonConvert.DeserializeObject<Layer>(json);
            init(layer);
            return layer;
        }
        private static void init(Layer layer)
        {
            handleList(layer);
            RecursionInit(layer.layers);
        }
        private static void handleList(Layer layer)
        {
            layer.eLayerKind = GetELayerKind(layer.layerKind,layer.name);
        }
        private static void RecursionInit(List<Layer> layers)
        {
            foreach (var layer in layers)
            {
                handleList(layer);
                if (layer.eLayerKind == ELayerKind.group || layer.eLayerKind == ELayerKind.canvas)
                {
                    RecursionInit(layer.layers);
                }
            }
        }
        // 自定义的部分
        private static ELayerKind GetELayerKind(string layerKind,string layerName)
        {
            switch (layerKind)
            {
                case "group":
                    return ELayerKind.group;
                case "canvas":
                    return ELayerKind.canvas;
                case "pixel":
                    return ELayerKind.pixel;
                case "smartObject":
                    return ELayerKind.smartObject;
                case "text":
                    return ELayerKind.text;
                default:
                    if (layerKind == null)
                    {
                        LogUtil.LogError("处理层级类型的时候出现了错误,根本没有设置layerking,层级名为"+layerName);
                        return ELayerKind.pixel;
                    }
                    LogUtil.LogError("处理层级类型的时候出现了错误,"+layerKind+"层级名为"+layerName);
                    
                    return ELayerKind.pixel;
            }
        }
    }
}