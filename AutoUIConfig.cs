using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;



namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class AutoUIConfig
    {
        public const string AutoUIConfigPath = "Assets/Scripts/Tools/Editor/AutoUI/AutoUIConfig.json";

        public static AutoUIConfigData config;
        public static void GetAutoUIConfigData()
        {
            if (!File.Exists(AutoUIConfigPath)){
                var err = new AutoUIException("AutoUIConfig.json不存在,请去AutoUIConfig.cs中进行设置");
                LogUtil.LogError(err);
                return;
            }
            string json = File.ReadAllText(AutoUIConfigPath);
            LogUtil.Log("json path:"+AutoUIConfigPath);
            LogUtil.Log("json:"+json);
            config= JsonConvert.DeserializeObject<AutoUIConfigData>(json);
        }

    }
    [System.Serializable]
    public class AutoUIConfigData
    {
        public TextConfigData text;
        public SpriteConfigData sprite;

        public PreviewSpriteSize previewSpriteSize;
    }

    [System.Serializable]
    public class TextConfigData{
        public string componentName;
        public string fontAssetPath;
        public float fontScale;
    }
    [System.Serializable]
    public class SpriteConfigData{
        public string spritePath;
    }
    [System.Serializable]
    public class PreviewSpriteSize{
        public int width;
        public int height;
    }
}