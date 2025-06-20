using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor.Localization.Editor;
using UnityEditor.Overlays;
using UnityEditor.SearchService;



namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class AutoUIConfig
    {
        public const string AutoUIConfigPath = "Assets/Scripts/Tools/Editor/AutoUI/AutoUIConfig.json";

        public static AutoUIConfigData config;
        public static void GetAutoUIConfigData()
        {
            if (!File.Exists(AutoUIConfigPath))
            {
                LogUtil.LogError("AutoUIConfig.json不存在,请去AutoUIConfig.cs中进行设置");
            }
            string json = File.ReadAllText(AutoUIConfigPath);
            LogUtil.Log("json path:" + AutoUIConfigPath);
            LogUtil.Log("json:" + json);
            config = JsonConvert.DeserializeObject<AutoUIConfigData>(json);
        }

    }
    public class AutoUIConfigData
    {
        [JsonProperty("default")]
        public DefaultConfig Default { get; set; }

        [JsonProperty("fontAssets")]
        public FontAssets FontAssets { get; set; }
    }
    // ----------------------------- Default -----------------------------

    public class DefaultConfig
    {
        [JsonProperty("buttonClickEffect")]
        public ButtonClickEffect ButtonClickEffect { get; set; }

        [JsonProperty("buttonComponent")]
        public ButtonComponent ButtonComponent { get; set; }

        [JsonProperty("scene")]
        public SceneInfo Scene { get; set; }

        [JsonProperty("prefab")]
        public PrefabInfo Prefab { get; set; }
        [JsonProperty("localization")]
        public Localization Localization { get; set; }
        [JsonProperty("layout")]
        public Assets.Scripts.Tools.Editor.AutoUI.Layout Layout { get; set; }
        [JsonProperty("font")]
        public Assets.Scripts.Tools.Editor.AutoUI.Font Font{ get; set; }
    }
    public class Font
    {
        [JsonProperty("enableCorrect")]
        public bool EnableCorrect { get; set; }
        [JsonProperty("correctValue")]
        public float CorrectValue { get; set; }
    }
    public class Layout
    {
        [JsonProperty("padding")]
        public int Padding { get; set; }
    }
    public class Localization
    {
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("isUseLocalization")]
        public bool IsUseLocalization { get; set; }
    }

    public class ButtonClickEffect
    {
        [JsonProperty("EnableClickEffect")]
        public bool EnableClickEffect { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("componentName")]
        public string ComponentName { get; set; }

        [JsonProperty("componentPath")]
        public string ComponentPath { get; set; }
    }

    public class ButtonComponent
    {
        [JsonProperty("useComponent")]
        public bool UseComponent { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("prefabPath")]
        public string PrefabPath { get; set; }
    }

    public class SceneInfo
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }
    }

    public class PrefabInfo
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    // ----------------------------- Font Assets -----------------------------

    public class FontAssets
    {
        [JsonProperty("default")]
        public FontAsset Default { get; set; }

        [JsonProperty("title")]
        public FontAsset Title { get; set; }

        [JsonProperty("supercell")]
        public FontAsset Supercell { get; set; }
    }

    public class FontAsset
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("materialPreset")]
        public MaterialPresets MaterialPreset { get; set; }
    }

    public class MaterialPresets
    {
        [JsonProperty("shadow")]
        public MaterialPreset Shadow { get; set; }

        [JsonProperty("yellow")]
        public MaterialPreset Yellow { get; set; }
    }

    public class MaterialPreset
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }
    }
}