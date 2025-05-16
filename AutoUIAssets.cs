// 动态的加载资源实在是太慢了。必须在最开始预加载一次，这是可行的。

using System.Collections.Generic;
using System.IO;
using Tuyoo.Render;
using UnityEditor;
using UnityEngine;

// 命名有点失误了，这里的sprite和assets混用了，虽然在这里这两者的意思差不多
namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public enum EFindAssetStatus
    {
        oneResult,
        manyResult,
        cantFind,
    }
    public class FindSpriteResult
    {
        public EFindAssetStatus status;
        public SpriteInfo oneResult;
        public List<SpriteInfo> manyResult;
    }
    public class SpriteInfo
    {
        public string path;
        public Sprite sprite;
    }
    public class AutoUIAssets
    {
        private static List<string> AssetsName = new List<string>();
        private static Dictionary<string, SpriteInfo> AssetsOnlyOneName = new Dictionary<string, SpriteInfo>();
        private static Dictionary<string, List<SpriteInfo>> AssetsManyName = new Dictionary<string, List<SpriteInfo>>();
        private static List<string> AssetsCantFind = new List<string>();
        public static void InitAssets(Layer layer)
        {
            AssetsOnlyOneName.Clear();
            AssetsManyName.Clear();
            AssetsName.Clear();
            AssetsCantFind.Clear();
            RecursionInit(layer.layers);
            foreach (var assetName in AssetsName)
            {
                if (IsSpriteExist(assetName)){
                   continue; 
                }
                var guids = AssetDatabase.FindAssets($"{assetName} t:Sprite");
                if (guids.Length == 0)
                {
                    AssetsCantFind.Add(assetName);
                    continue;
                }
                if (guids.Length == 1)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                    AssetsOnlyOneName.Add(assetName, new SpriteInfo { path = path, sprite = sprite });
                }
                if (guids.Length > 1)
                {
                    List<SpriteInfo> sprites = new List<SpriteInfo>();
                    foreach (var guid in guids)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        // 进行检查，因为搜索的结果是模糊搜索，所以需要对名字进行查看，是不是精确搜索
                        string filename=Path.GetFileNameWithoutExtension(path);
                        if (filename!=assetName){
                            continue;
                        }
                        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                        sprites.Add(new SpriteInfo { path = path, sprite = sprite });
                    }
                    if(sprites.Count==0){
                        // 并不是错误，很正常
                        AssetsCantFind.Add(assetName);
                    }
                    if (sprites.Count == 1){
                        AssetsOnlyOneName.Add(assetName, sprites[0]);
                    }
                    if (sprites.Count > 1){
                        AssetsManyName.Add(assetName, sprites); 
                    }
                }
                if (guids==null){
                    LogUtil.Log("AssetsName is null");
                }

            }
        }
        private static void RecursionInit(List<Layer> layers)
        {
            foreach (var layer in layers)
            {
                if (layer.eLayerKind == ELayerKind.pixel)
                {
                    AssetsName.Add(layer.name);
                }
                // todo : 添加对智能对象的支持
                if (layer.eLayerKind == ELayerKind.group)
                {
                    RecursionInit(layer.layers);
                }
            }
        }

        public static FindSpriteResult GetSprite(string name)
        {
            // 大多数情况下都是一个结果
            if (AssetsOnlyOneName.ContainsKey(name))
            {
                return new FindSpriteResult { status = EFindAssetStatus.oneResult, oneResult = AssetsOnlyOneName[name] };
            }
            // 少数情况下是多个结果
            if (AssetsManyName.ContainsKey(name))
            {
                return new FindSpriteResult { status = EFindAssetStatus.manyResult, manyResult = AssetsManyName[name] };
            }
            // 极少数情况下是找不到的
            if (AssetsCantFind.Contains(name))
            {
                return new FindSpriteResult { status = EFindAssetStatus.cantFind };
            }
            // 逻辑错误
            LogUtil.LogError("找不到这个预加载的sprite  名字为:" + name);
            return null;
        }
        private static bool IsSpriteExist(string name){
            if (AssetsOnlyOneName.ContainsKey(name)){
                return true;
            }
            if (AssetsManyName.ContainsKey(name)){
                return true;
            }
            return false;
        }


    }

}