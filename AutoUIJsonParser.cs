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
            layer.eLayerKind = GetELayerKind(layer.layerKind);
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
        private static ELayerKind GetELayerKind(string layerKind)
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
                    LogUtil.LogError("处理层级类型的时候出现了错误");
                    return ELayerKind.pixel;
            }
        }
    }
}