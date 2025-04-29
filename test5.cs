#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Tuyoo.Render;

// 日志工具类
public static class LogUtil
{
    private static string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "unity_ui_log.txt");
    private static bool isFirstWrite = true;

    public static void Log(string message)
    {
        string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logMessage = $"[{timeStamp}] {message}";

        try
        {
            // 如果是第一次写入，清空文件
            if (isFirstWrite)
            {
                File.WriteAllText(logPath, $"=== Unity UI 生成日志 ===\n{logMessage}\n");
                isFirstWrite = false;
            }
            else
            {
                File.AppendAllText(logPath, logMessage + "\n");
            }
        }
        catch (Exception e)
        {
            // 如果文件写入失败，改用控制台输出
            Console.WriteLine($"[日志系统错误] 无法写入日志文件: {e.Message}");
            Console.WriteLine(logMessage);
        }
    }

    public static void LogWarning(string message)
    {
        Log($"[警告] {message}");  // 直接调用 Log，不再递归
    }

    public static void LogError(string message)
    {
        Log($"[错误] {message}");  // 直接调用 Log，不再递归
    }
}
[System.Serializable]
public class AutoUIConfig
{
    public string ButtonPath;
}


[System.Serializable]
public class ComponentData
{
    public string name;
    public Dictionary<string, object> parameters;
}

[System.Serializable]
public class SmartObjectData
{
    public string fileReference;
    public SizeData size;
}

[System.Serializable]
public class SizeData
{
    public string _obj;
    public float width;
    public float height;
}
[System.Serializable]
public class PixelData{
    public string name;
}
[System.Serializable]
public class ColorData
{
    public float r;
    public float g;
    public float b;
}

[System.Serializable]
public class LayerData
{
    public string name;
    public int id;
    public bool visible;
    public float opacity;
    public string blendMode;
    public int layerKind;
    public List<ComponentData> components;
    public RectTransformData rectTransform;
    public List<LayerData> list;
    public string type;
    public string text;
    public float fontSize;
    public ColorData color;
    public string textAlign;
    public SmartObjectData smartObject;
}

[System.Serializable]
public class RectTransformData
{
    public string type;
    public List<AnchorPoint> anchor;
    public List<float> pivot;
    public float posX;
    public float posY;
    public float width;
    public float height;
    public float right;
    public float bottom;
    public float left;
    public float top;
}


[System.Serializable]
public class AnchorPoint
{
    public float x;
    public float y;
}

[System.Serializable]
public class RootData
{
    public List<LayerData> list;
    public string name;
    public string type;
    public float width;
    public float height;
}

// UI层处理策略接口
public interface IUILayerProcessorStrategy
{
    bool CanProcess(LayerData layer);
    void Process(GameObject layerObj, LayerData layer);
}

// 文本层处理策略
public class TextLayerProcessorStrategy : IUILayerProcessorStrategy
{
    public bool CanProcess(LayerData layer)
    {
        return layer.type == "text";
    }

    public void Process(GameObject layerObj, LayerData layer)
    {
        Text text = layerObj.AddComponent<Text>();
        text.text = layer.text;
        text.fontSize = Mathf.RoundToInt(layer.fontSize);
        text.color = new Color(layer.color.r / 255f, layer.color.g / 255f, layer.color.b / 255f);
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 设置文本对齐方式
        switch (layer.textAlign)
        {
            case "center":
                text.alignment = TextAnchor.MiddleCenter;
                break;
            case "left":
                text.alignment = TextAnchor.MiddleLeft;
                break;
            case "right":
                text.alignment = TextAnchor.MiddleRight;
                break;
        }
    }
}

// 智能对象层处理策略
public class SmartObjectLayerProcessorStrategy : IUILayerProcessorStrategy
{
    private string jsonFolderPath;

    public SmartObjectLayerProcessorStrategy(string jsonFolderPath)
    {
        this.jsonFolderPath = jsonFolderPath;
    }

    public bool CanProcess(LayerData layer)
    {
        return layer.type == "smartObject" && layer.smartObject != null;
    }

    public void Process(GameObject layerObj, LayerData layer)
    {
        Image image = layerObj.GetComponent<Image>();
        if (image == null)
        {
            image = layerObj.AddComponent<Image>();
        }

        //string imagePath = Path.Combine(jsonFolderPath, layer.smartObject.fileReference);
        string imagePath = jsonFolderPath + "/" + layer.smartObject.fileReference;
        if (File.Exists(imagePath))
        {
            // 将图片文件复制到Assets目录下
            string destPath = "Assets/Images/"+layer.smartObject.fileReference;
            string destDir = Path.GetDirectoryName(destPath);
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            File.Copy(imagePath, destPath, true);
            AssetDatabase.Refresh();

            // 设置图片导入设置
            TextureImporter importer = AssetImporter.GetAtPath(destPath) as TextureImporter;
            if (importer != null)
            {
                LogUtil.Log($"处理图片导入设置: {destPath}");
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;

                // 如果这个图片有9slice组件，预先设置为支持九宫格
                var sliceComponent = layer.components?.FirstOrDefault(c => c.name == "9slice");
                if (sliceComponent != null && sliceComponent.parameters != null)
                {
                    float top = Convert.ToSingle(sliceComponent.parameters["top"]);
                    float right = Convert.ToSingle(sliceComponent.parameters["right"]);
                    float bottom = Convert.ToSingle(sliceComponent.parameters["bottom"]);
                    float left = Convert.ToSingle(sliceComponent.parameters["left"]);

                    LogUtil.Log($"设置九宫格边框 - Left: {left}, Bottom: {bottom}, Right: {right}, Top: {top}");
                    importer.spriteBorder = new Vector4(left, bottom, right, top);
                }

                importer.SaveAndReimport();
            }

            // 加载图片并设置
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(destPath);
            if (sprite != null)
            {
                image.sprite = sprite;
                image.preserveAspect = true;

                // 如果有9slice组件，设置图片类型为九宫格
                if (layer.components?.Any(c => c.name == "9slice") == true)
                {
                    LogUtil.Log($"将图片 {layerObj.name} 设置为九宫格类型");
                    image.type = Image.Type.Sliced;
                }
            }
            else
            {
                LogUtil.LogError($"无法加载图片: {destPath}");
            }
        }
        else
        {
            LogUtil.LogError($"找不到图片文件: {imagePath}");
        }
    }
}

// 像素层处理策略
public class PixelLayerProcessorStrategy : IUILayerProcessorStrategy
{
    private string jsonFolderPath;

    public PixelLayerProcessorStrategy(string jsonFolderPath)
    {
        this.jsonFolderPath = jsonFolderPath;
    }

    public bool CanProcess(LayerData layer)
    {
        return layer.type == "pixel";
    }

    public void Process(GameObject layerObj, LayerData layer)
    {
        LogUtil.Log("进行一次layer的处理, layer:"+layer);
        Image image = layerObj.GetComponent<Image>();
        if (image == null)
        {
            image = layerObj.AddComponent<Image>();
        }

        //string imagePath = Path.Combine(jsonFolderPath, layer.smartObject.fileReference);
        string imagePath = jsonFolderPath + "/" + layer.name+".png";
        if (File.Exists(imagePath))
        {
            // 将图片文件复制到Assets目录下
            string destPath = Path.Combine("Assets/Images", layer.name+".png");
            string destDir = Path.GetDirectoryName(destPath);
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }
            File.Copy(imagePath, destPath, true);
            AssetDatabase.Refresh();

            // 设置图片导入设置
            TextureImporter importer = AssetImporter.GetAtPath(destPath) as TextureImporter;
            if (importer != null)
            {
                LogUtil.Log($"处理图片导入设置: {destPath}");
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;

                // 如果这个图片有9slice组件，预先设置为支持九宫格
                var sliceComponent = layer.components?.FirstOrDefault(c => c.name == "9slice");
                if (sliceComponent != null && sliceComponent.parameters != null)
                {
                    float top = Convert.ToSingle(sliceComponent.parameters["top"]);
                    float right = Convert.ToSingle(sliceComponent.parameters["right"]);
                    float bottom = Convert.ToSingle(sliceComponent.parameters["bottom"]);
                    float left = Convert.ToSingle(sliceComponent.parameters["left"]);

                    LogUtil.Log($"设置九宫格边框 - Left: {left}, Bottom: {bottom}, Right: {right}, Top: {top}");
                    importer.spriteBorder = new Vector4(left, bottom, right, top);
                }

                importer.SaveAndReimport();
            }

            // 加载图片并设置
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(destPath);
            if (sprite != null)
            {
                image.sprite = sprite;
                image.preserveAspect = true;

                // 如果有9slice组件，设置图片类型为九宫格
                if (layer.components?.Any(c => c.name == "9slice") == true)
                {
                    LogUtil.Log($"将图片 {layerObj.name} 设置为九宫格类型");
                    image.type = Image.Type.Sliced;
                }
            }
            else
            {
                LogUtil.LogError($"无法加载图片: {destPath}");
            }
        }
        else
        {
            LogUtil.LogError($"找不到图片文件: {imagePath}");
        }
    }
}

// 组件处理器接口
public interface IComponentProcessor
{
    bool CanProcess(ComponentData component);
    void Process(GameObject gameObject, ComponentData component);
}

// 按钮组件处理器
public class ButtonComponentProcessor : IComponentProcessor
{
    public bool CanProcess(ComponentData component)
    {
        return component.name == "button";
    }

    public void Process(GameObject gameObject, ComponentData component)
    {
        GameObject ButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(JsonToUIPrefabCreator.autoUIConfig.ButtonPath);
        GameObject button = GameObject.Instantiate(ButtonPrefab, gameObject.transform);

        if (component.parameters.TryGetValue("type", out object typeObj))
        {
            string type = typeObj.ToString();
            // 可以根据type设置不同的按钮样式
            if (type == "simple")
            {
                // 设置简单按钮样式
                if (component.parameters.TryGetValue("color", out object colorObj))
                {
                    ButtonTemplate buttonTemplate = button.GetComponent<ButtonTemplate>();
                    string color = colorObj.ToString();
                    // 1. 上色
                    ButtonColor buttonColor;
                    switch (color)
                    {
                        case "red":
                            buttonColor = ButtonColor.Red;
                            break;
                        case "yellow":
                            buttonColor = ButtonColor.Yellow;
                            break;
                        case "green":
                            buttonColor = ButtonColor.Green;
                            break;
                        case "blue":
                            buttonColor = ButtonColor.Blue;
                            break;
                        default:
                            buttonColor = ButtonColor.Grey;
                            break;
                    }
                    buttonTemplate.SetButtonColor(buttonColor);
                    // 简单的样式，则需要将文字复制过去
                    Text text = gameObject.GetComponent<Text>();
                    buttonTemplate.SetButtonNameSingleByValue(text.text);

                    // 清除原本的文本内容
                    text.gameObject.SetActive(false);
                }
            }
            else if (type == "complex")
            {
                // 设置复杂按钮样式
                if (component.parameters.TryGetValue("title", out object titleObj))
                {
                    string title = titleObj.ToString();
                    // 设置按钮标题
                }
            }
        }
    }
}

// 九宫格图片处理器
public class NineSliceComponentProcessor : IComponentProcessor
{
    public bool CanProcess(ComponentData component)
    {
        return component.name == "9slice";
    }

    public void Process(GameObject gameObject, ComponentData component)
    {
        LogUtil.Log($"开始处理九宫格组件: {gameObject.name}");

        try
        {
            Image image = gameObject.GetComponent<Image>();
            if (image == null)
            {
                LogUtil.Log("添加Image组件");
                image = gameObject.AddComponent<Image>();
            }

            if (component.parameters == null)
            {
                throw new Exception("九宫格参数为空");
            }

            if (!component.parameters.ContainsKey("top") ||
                !component.parameters.ContainsKey("right") ||
                !component.parameters.ContainsKey("bottom") ||
                !component.parameters.ContainsKey("left"))
            {
                throw new Exception("缺少必要的九宫格参数(top/right/bottom/left)");
            }

            if (image.sprite == null)
            {
                throw new Exception("Image组件没有设置Sprite");
            }

            // 设置图片类型为九宫格
            image.type = Image.Type.Sliced;

            // 获取九宫格的四个边距值
            float top = Convert.ToSingle(component.parameters["top"]);
            float right = Convert.ToSingle(component.parameters["right"]);
            float bottom = Convert.ToSingle(component.parameters["bottom"]);
            float left = Convert.ToSingle(component.parameters["left"]);

            LogUtil.Log($"九宫格参数 - Left: {left}, Bottom: {bottom}, Right: {right}, Top: {top}");

            // 获取sprite的资源路径
            string spritePath = AssetDatabase.GetAssetPath(image.sprite);
            if (string.IsNullOrEmpty(spritePath))
            {
                throw new Exception("无法获取Sprite资源路径");
            }
            LogUtil.Log($"Sprite路径: {spritePath}");

            // 通过TextureImporter设置border
            TextureImporter importer = AssetImporter.GetAtPath(spritePath) as TextureImporter;
            if (importer == null)
            {
                throw new Exception($"无法获取TextureImporter: {spritePath}");
            }

            LogUtil.Log("开始设置TextureImporter...");

            // 确保是Sprite类型
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;

            // 设置border
            Vector4 border = new Vector4(left, bottom, right, top);
            importer.spriteBorder = border;

            LogUtil.Log($"设置的border值: {importer.spriteBorder}");

            // 应用设置
            EditorUtility.SetDirty(importer);
            AssetDatabase.SaveAssets();
            importer.SaveAndReimport();

            // 验证设置是否成功
            TextureImporter verifyImporter = AssetImporter.GetAtPath(spritePath) as TextureImporter;
            if (verifyImporter != null)
            {
                LogUtil.Log($"验证border值: {verifyImporter.spriteBorder}");
            }

            // 重新获取更新后的sprite
            AssetDatabase.Refresh();
            Sprite updatedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (updatedSprite != null)
            {
                image.sprite = updatedSprite;
                LogUtil.Log($"Sprite更新后的border: {updatedSprite.border}");
            }
            else
            {
                throw new Exception("无法重新加载Sprite");
            }
        }
        catch (Exception e)
        {
            LogUtil.LogError($"处理九宫格组件时出错: {e.Message}\n{e.StackTrace}");
        }
    }
}

// 组件处理器工厂
public class ComponentProcessorFactory
{
    private static Dictionary<string, IComponentProcessor> processors = new Dictionary<string, IComponentProcessor>();

    static ComponentProcessorFactory()
    {
        LogUtil.Log("注册组件处理器...");
        RegisterProcessor("button", new ButtonComponentProcessor());
        RegisterProcessor("9slice", new NineSliceComponentProcessor());
        LogUtil.Log($"已注册的处理器: {string.Join(", ", processors.Keys)}");
    }

    public static void RegisterProcessor(string name, IComponentProcessor processor)
    {
        processors[name] = processor;
        LogUtil.Log($"注册处理器: {name}");
    }

    public static IComponentProcessor GetProcessor(string name)
    {
        if (processors.TryGetValue(name, out var processor))
        {
            LogUtil.Log($"获取处理器: {name}");
            return processor;
        }
        LogUtil.LogWarning($"未找到处理器: {name}");
        return null;
    }
}

// 处理的核心调用层
public class UILayerProcessor
{
    private static string jsonFolderPath;
    private static List<IUILayerProcessorStrategy> strategies;

    static UILayerProcessor()
    {
        strategies = new List<IUILayerProcessorStrategy>();
        RegisterStrategy(new TextLayerProcessorStrategy());
    }

    public static void SetJsonFolderPath(string path)
    {
        jsonFolderPath = path;
        RegisterStrategy(new SmartObjectLayerProcessorStrategy(jsonFolderPath));
    }

    public static void RegisterStrategy(IUILayerProcessorStrategy strategy)
    {
        if (!strategies.Contains(strategy))
        {
            strategies.Add(strategy);
        }
    }

    // 最关键的处理函数
    public static void ProcessLayer(GameObject layerObj, LayerData layer)
    {
        LogUtil.Log($"开始处理层: {layer.name}");

        // 处理基本层属性 transform
        ProcessLayerProperties(layerObj, layer);

        // 先处理特定类型的层 比如image，text
        // 因为组件（如9slice）可能依赖于这些基础组件
        foreach (var strategy in strategies)
        {
            if (strategy.CanProcess(layer))
            {
                LogUtil.Log($"使用策略处理层 {layer.name}: {strategy.GetType().Name}");
                strategy.Process(layerObj, layer);
                break;
            }
        }

        // 再处理组件 比如9slice button
        ProcessComponents(layerObj, layer);
    }

    private static void ProcessLayerProperties(GameObject layerObj, LayerData layer)
    {
        // 设置可见性
        layerObj.SetActive(layer.visible);

        // 设置透明度（如果有CanvasGroup组件）
        var canvasGroup = layerObj.GetComponent<CanvasGroup>();
        if (canvasGroup == null && layer.opacity < 1)
        {
            canvasGroup = layerObj.AddComponent<CanvasGroup>();
        }
        if (canvasGroup != null)
        {
            canvasGroup.alpha = layer.opacity;
        }
    }

    private static void ProcessComponents(GameObject layerObj, LayerData layer)
    {
        if (layer.components == null)
        {
            LogUtil.Log($"GameObject {layerObj.name} 没有组件配置");
            return;
        }

        LogUtil.Log($"处理 {layerObj.name} 的组件，组件数量: {layer.components.Count}");
        foreach (var component in layer.components)
        {
            LogUtil.Log($"正在处理组件: {component.name}");
            if (component.parameters != null)
            {
                LogUtil.Log($"组件参数: {JsonConvert.SerializeObject(component.parameters)}");
            }

            var processor = ComponentProcessorFactory.GetProcessor(component.name);
            if (processor != null)
            {
                processor.Process(layerObj, component);
            }
        }
    }
}

public class JsonToUIPrefabCreator : EditorWindow
{
    private static string prefabPath = "Assets/Prefabs/UI/test.prefab";
    private static string jsonFolderPath;
    public static AutoUIConfig autoUIConfig;

    //  核心逻辑代码
    [MenuItem("Tools/TestAutoUI")]
    public static void CreateUIPrefabFromJson()
    {
        LogUtil.Log("=== 开始生成UI预制体 ===");

        // 打开文件夹选择对话框
        jsonFolderPath = EditorUtility.OpenFolderPanel("选择包含data.json的文件夹", "", "");
        if (string.IsNullOrEmpty(jsonFolderPath))
        {
            LogUtil.Log("用户取消选择文件夹");
            return;
        }

        string jsonPath = Path.Combine(jsonFolderPath, "data.json");
        if (!File.Exists(jsonPath))
        {
            LogUtil.LogError("未找到data.json文件！");
            return;
        }

        try
        {
            // 将所有的图片保存起来
            // 这里将来设置一个用来选择保存到哪里的功能
            CopyImagesToAssetFolder(jsonFolderPath);
            // 读取并解析JSON
            string jsonContent1 = File.ReadAllText(jsonPath);
            RootData rootData = JsonConvert.DeserializeObject<RootData>(jsonContent1);

            string jsonFilePath = "Assets\\Scripts\\Tools\\Editor\\AutoUI\\testAutoUIConfig.json";
            string jsonContent2 = File.ReadAllText(jsonFilePath);
            autoUIConfig = JsonConvert.DeserializeObject<AutoUIConfig>(jsonContent2);
            LogUtil.Log($"成功解析JSON数据，画布大小: {rootData.width}x{rootData.height}");

            // 创建UI预制体
            GameObject canvasObj = CreateCanvasWithData(rootData);

            // 确保目标路径的文件夹存在
            CreateTargetDirectory();

            // 保存为预制体并清理
            SavePrefabAndCleanup(canvasObj);

            LogUtil.Log($"UI预制体已生成！路径: {prefabPath}");
        }
        catch (Exception e)
        {
            LogUtil.LogError($"生成UI预制体时出错: {e.Message}");
        }
    }
    //  根据解析过后的json数据，生成一个物体
    private static GameObject CreateCanvasWithData(RootData rootData)
    {
        // 创建Canvas
        GameObject canvasObj = new GameObject("UICanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 设置Canvas大小
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(rootData.width, rootData.height);

        // 处理所有层
        ProcessLayers(rootData.list, canvasObj.transform);

        return canvasObj;
    }

    //  处理所有层,核心逻辑代码
    private static void ProcessLayers(List<LayerData> layers, Transform parent)
    {
        if (layers == null) return;

        foreach (var layer in layers)
        {
            GameObject layerObj = new GameObject(layer.name);
            layerObj.transform.SetParent(parent);
            RectTransform rectTransform = layerObj.AddComponent<RectTransform>();

            // 设置位置和大小
            if (layer.rectTransform != null)
            {
                rectTransform.anchoredPosition = new Vector2(layer.rectTransform.posX, layer.rectTransform.posY);
                rectTransform.sizeDelta = new Vector2(layer.rectTransform.width, layer.rectTransform.height);

                // 设置锚点
                if (layer.rectTransform.anchor != null && layer.rectTransform.anchor.Count == 2)
                {
                    rectTransform.anchorMin = new Vector2(layer.rectTransform.anchor[0].x, layer.rectTransform.anchor[0].y);
                    rectTransform.anchorMax = new Vector2(layer.rectTransform.anchor[1].x, layer.rectTransform.anchor[1].y);
                }
                // 设置关键的位置属性
                switch (layer.rectTransform.type)
                {
                    case "dynamic":
                        rectTransform.offsetMin = new Vector2(layer.rectTransform.left, layer.rectTransform.bottom);
                        rectTransform.offsetMax = new Vector2(-layer.rectTransform.right, -layer.rectTransform.top);


                        break;
                    case "static":
                        rectTransform.anchoredPosition = new Vector2(layer.rectTransform.posX, layer.rectTransform.posY);
                        rectTransform.sizeDelta = new Vector2(layer.rectTransform.width, layer.rectTransform.height);
                        break;
                }
                // 设置轴心点
                if (layer.rectTransform.pivot != null && layer.rectTransform.pivot.Count == 2)
                {
                    rectTransform.pivot = new Vector2(layer.rectTransform.pivot[0], layer.rectTransform.pivot[1]);
                }
            }

            // 处理不同类型的层
            UILayerProcessor.SetJsonFolderPath(jsonFolderPath);

            /// 关键处理
            UILayerProcessor.ProcessLayer(layerObj, layer);

            // 递归处理子层
            if (layer.list != null && layer.list.Count > 0)
            {
                ProcessLayers(layer.list, layerObj.transform);
            }
        }
    }

    // 检查或者新建一个perfabPath，用来存放prefab
    private static void CreateTargetDirectory()
    {
        string directory = Path.GetDirectoryName(prefabPath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    // 将物体以prefab的形式保存下来
    private static void SavePrefabAndCleanup(GameObject target)
    {
        PrefabUtility.SaveAsPrefabAsset(target, prefabPath);
        DestroyImmediate(target);
        AssetDatabase.Refresh();
    }

    private static void CopyImagesToAssetFolder(string sourceFolderPath)
    {
        string imageTargetFolderPath = Path.Combine("Assets", "Images");
        if (!Directory.Exists(imageTargetFolderPath))
        {
            Directory.CreateDirectory(imageTargetFolderPath);
        }
        
        // 获取所有的图片文件（可以根据实际需求扩展文件类型）
        string[] imageExtensions = { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.tga" };
        foreach (string extension in imageExtensions)
        {
            string[] imageFiles = Directory.GetFiles(sourceFolderPath, extension);
            foreach (string imageFile in imageFiles)
            {
                string fileName = Path.GetFileName(imageFile);
                string destFilePath = Path.Combine(imageTargetFolderPath, fileName);

                try
                {
                    File.Copy(imageFile, destFilePath, true);
                    LogUtil.Log($"图片文件已复制: {fileName}");
                }
                catch (Exception e)
                {
                    LogUtil.LogError($"复制图片文件时出错: {e.Message}");
                }
            }
        }

        AssetDatabase.Refresh();
    }

}

#endif