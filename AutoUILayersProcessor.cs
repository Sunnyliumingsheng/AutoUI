
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityFramework;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    // 对每个层级进行一个简单的处理
    public class AutoUILayersProcessor
    {
        public static void LayerProcessor(in Layer layer, ref GameObject layerGameObject)
        {
            switch (layer.eLayerKind)
            {
                case ELayerKind.smartObject:
                    SmartObjectLayerProcessor(in layer, ref layerGameObject);
                    break;
                case ELayerKind.pixel:
                    PixelLayerProcessor(in layer, ref layerGameObject);
                    break;
                case ELayerKind.text:
                    TextLayerProcessor(in layer, ref layerGameObject);
                    break;
                default:
                    AutoUIException err = new AutoUIException("逻辑错误，这里不应该有其他图层进入" + layer.eLayerKind);
                    LogUtil.LogError(err);
                    break;
            }
        }


        public static void SmartObjectLayerProcessor(in Layer layer, ref GameObject layerGameObject)
        {
            // 智能对象的图片依赖引用名称
            string imageAssetPath= AutoUIImagesImportProcessor.ParseImageName(layer.smartObjectLayerData.fileReference);
            if(!AutoUIImagesImportProcessor.IsImageExist(imageAssetPath)){
                AutoUIException err = new AutoUIException("智能对象的图片依赖引用名称不存在 fileReference:"+layer.smartObjectLayerData.fileReference+"   imageAssetPath:"+imageAssetPath);
                LogUtil.LogError(err);
                return;
            }
            Sprite sprite = Resources.Load<Sprite>(imageAssetPath);
            // 添加image
            Image image = layerGameObject.AddComponent<Image>();
            image.sprite = sprite;
        }

        public static void PixelLayerProcessor(in Layer layer, ref GameObject layerGameObject)
        {
            /* 请看Main中的导入步骤的注释
            string imageAssetPath= AutoUIImagesImportProcessor.ParseImageName(layer.name)+".png";
            if(!AutoUIImagesImportProcessor.IsImageExist(imageAssetPath)){
                AutoUIException err = new AutoUIException("像素图层的图片依赖引用名称不存在name:"+layer.name+"   imageAssetPath:"+imageAssetPath);
                LogUtil.LogError(err);
                return; 
            }
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(imageAssetPath);
            if(sprite == null){
                AutoUIException err = new AutoUIException("找不到这个sprite  路径为:"+imageAssetPath);
                LogUtil.LogError(err);
                return;
            }
            */
            // 利用unity的搜索功能，看看能找到几个这个名字的Sprite
            string[] guids = AssetDatabase.FindAssets($"{layer.name} t:Sprite");
            if(guids.Length == 0){
               LogUtil.LogWarning("找不到这个sprite  名字为:"+layer.name);
               IProduct gui= GUIManager.CreateGUINotSelectSprite();
               gui.PutOnLine0();
               
               AutoUIEventManager.GUINotSelectSpriteEvent.Subscribe((sender,args)=>{
                    if(args.SkipImportImage){
                        LogUtil.Log("跳过导入图片");
                    }
                    else{
                        LogUtil.Log("这里是路径"+args.ImageSrcPath+args.ImageDestPath);
                    }
                    return;
               });

            }
            if (guids.Length==1){
                LogUtil.Log("找到这个sprite  名字为:"+layer.name);
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                Texture2D preview = AssetPreview.GetAssetPreview(sprite);
            if (preview != null)
            {
                GUILayout.Label(preview, GUILayout.Width(64), GUILayout.Height(64));
                GUILayout.Label("图片路径: " + path);
                GUILayout.Button("确认是这个");
            }

            }
            if (guids.Length>1){
                LogUtil.Log("找到多个sprite"); 
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                Texture2D preview = AssetPreview.GetAssetPreview(sprite);
            if (preview != null)
            {
                GUILayout.Label(preview, GUILayout.Width(64), GUILayout.Height(64));
                GUILayout.Label("图片路径: " + path);
                GUILayout.Button("确认是这个");
            }
            }
            
            
            // 添加image
            Image image = layerGameObject.AddComponent<Image>();
            //image.sprite = sprite;
        }
        public static void TextLayerProcessor(in Layer layer, ref GameObject layerGameObject)
        {
            // 第一部分，添加TMP
            TextMeshProUGUI tmp = layerGameObject.AddComponent<TextMeshProUGUI>();
            tmp.text = layer.textLayerData.text;
            // todo : 寻找到一个合适的字体转化关系函数。并进行使用
            tmp.fontSize = AutoUIUtil.PSTextSizeToUnityTMPFontSize(Mathf.RoundToInt(layer.textLayerData.fontSize));
            TMP_FontAsset tmpFontAsset = AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(AutoUIConfig.config.text.fontAssetPath);
            if (tmpFontAsset == null){
                LogUtil.LogError("找不到字体资源 路径为:"+AutoUIConfig.config.text.fontAssetPath);
                return;
            }
            tmp.font = tmpFontAsset;
            tmp.color = new Color(
                layer.textLayerData.color.r / 255f,
                layer.textLayerData.color.g / 255f,
                layer.textLayerData.color.b / 255f
                );
            var localizationTextTMP= layerGameObject.AddComponent<LocalizationText_TMP>();
            // 使用反射来给私有成员mLabel赋值
            var field=typeof(LocalizationText_TMP).GetField("mLabel",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(localizationTextTMP,tmp);
            EditorUtility.SetDirty(localizationTextTMP);

        }
        



    }
}