using UnityEditor;
using UnityEngine;
using Assets.Scripts.Tools.Editor.AutoUI;
using System.IO;
using System.Collections.Generic;


public class TestAutoUIParseComponent : EditorWindow
{
    [MenuItem("Tools/TestAutoUIParseComponent")]
    public static void Main()
    {
        LogUtil.Log("=== AutoUI start ===");
        LogUtil.ClearLogFile();
        string selectedFolderPath = AutoUIFile.SelectFolderPath();
        if (string.IsNullOrEmpty(selectedFolderPath))
        {
            LogUtil.LogError("解析终止，因为未选择有效的文件夹路径。");
            return;
        }
        string selectedJsonPath = selectedFolderPath + "/data.json";
        if (!AutoUIFile.IsJsonFileExist(selectedFolderPath))
        {
            LogUtil.LogError("解析终止，因为未选择有效的 JSON 文件路径。");
            return;
        }
        AutoUIConfig.GetAutoUIConfigData();
        LogUtil.Log("=== 开始解析json ===");
        string json = File.ReadAllText(selectedJsonPath);
        Layer layers = LayerJsonParser.ParseFromJson(json);
        LogUtil.Log("=== 开始验证 ===");
        递归图层(layers.layers);

    }
    private static void 递归图层(List<Layer> layers)
    {
        foreach (var layer in layers)
        {
            if (layer.eLayerKind == ELayerKind.group)
            {
                递归图层(layer.layers);
            }
            if (layer.components != null)
            {
                LogUtil.Log("层级名" + layer.name + "层级种类" + layer.eLayerKind);
                foreach (var component in layer.components)
                {
                    LogUtil.Log("组件名" + component.name);
                    LogUtil.Log("组件属性" + component.parameters.ToString());
                    foreach (var kvp in component.parameters)
                    {
                        string valueStr = kvp.Value != null ? kvp.Value.ToString() : "null";
                        LogUtil.Log($"Key = {kvp.Key}, Value = {valueStr} (Type = {kvp.Value?.GetType().Name ?? "null"})");
                    }
                }
            }
        }
    }
}

