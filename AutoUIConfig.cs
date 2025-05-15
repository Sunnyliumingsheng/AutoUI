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
            if (!File.Exists(AutoUIConfigPath))
            {
                var err = new AutoUIException("AutoUIConfig.json不存在,请去AutoUIConfig.cs中进行设置");
                LogUtil.LogError(err);
                return;
            }
            string json = File.ReadAllText(AutoUIConfigPath);
            LogUtil.Log("json path:" + AutoUIConfigPath);
            LogUtil.Log("json:" + json);
            config = JsonConvert.DeserializeObject<AutoUIConfigData>(json);
        }
        // 这个是一个字典，对第二位的字符串进行处理
        public static Dictionary<string,string> SpriteNameProject2SP=new Dictionary<string, string>(){
            {"Alliance","Alliance"},
            {"Archive","Archive"},
            {"BeiBao","Bag"},
            {"Battle","Battle"},
            {"City","City"},
            {"Cmn","Common"},
            {"Guide","Dialog"},
            {"Equip","Equip"},
            {"Head","Head"},
            {"Hero","Hero"},
            {"Icon","Icon"},
            {"Loading","Loading"},
            {"Main","Main"},
            {"Building","MainCity"},
            {"technology","Technology"},
            {"Mission","Mission"},
            {"Store","Store"},
            {"Itelligence","WatchTower"},
            {"World","World"}
        };
        // 特殊的文件夹，CommonBg BigImage 
        public static Dictionary<string,string> SpriteNameProject1=new Dictionary<string, string>(){
            {"SP","Sprites"},
            {"HUD","HUD"},
        };
    }
    [System.Serializable]
    public class AutoUIConfigData
    {
        public TextConfigData text;
        public SpriteConfigData sprite;

        public PreviewSpriteSize previewSpriteSize;
        public FontMaterialPath fontMaterialPath;
    }

    [System.Serializable]
    public class TextConfigData
    {
        public string componentName;
        public string fontAssetPath;
        public float fontScale;
    }
    [System.Serializable]
    public class SpriteConfigData
    {
        public string spritePath;
        public string scenePath;
    }
    [System.Serializable]
    public class PreviewSpriteSize
    {
        public int width;
        public int height;
    }
    [System.Serializable]
    public class FontMaterialPath
    {
        public string miaobian;
    }
}