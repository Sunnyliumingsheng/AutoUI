using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    class AutoUIGroupLayerProcessor
    {
        public static List<string> ExistPrefabNames = new List<string>();
        public static void ClearExistPrefabNames()
        {
            ExistPrefabNames.Clear();
        }

        public static void GroupLayerProcessor(in Layer layer, ref GameObject newGameObject)
        {
            /////// 基本处理


            /////// 处理组件
            if (AutoUIUtil.IsComponentExist(in layer, "button"))
            {
                var button = newGameObject.AddComponent<UnityEngine.UI.Button>();
                if (AutoUIConfig.config.Default.ButtonClickEffect.EnableClickEffect)
                {// 如果使用点击效果
                    newGameObject.AddComponent<UnityFramework.ButtonClickEffect>();
                }
            }

        }
        public static bool IsThisGroupAPrefab(in Layer layer)
        {
            if (layer.components != null)
            {
                foreach (var component in layer.components)
                {
                    if (component.name == "prefab")
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static string GetPrefabName(in Layer layer)
        {
            if (layer.components != null)
            {
                foreach (var component in layer.components)
                {
                    if (component.name == "prefab")
                    {
                        if (component.parameters != null)
                        {
                            component.parameters.TryGetValue("name", out object name);
                            string stringName = name as string;
                            if (stringName == null)
                            {
                                LogUtil.LogError("prefab组件的name参数不是string类型");
                            }
                            return stringName;
                        }
                        LogUtil.LogError("错误使用GetPrefabName,检查不到prefab组件的name参数");
                        return "";
                    }
                }
            }
            LogUtil.LogError("错误使用GetPrefabName,检查不到有解析的layer有prefab组件标记");
            return "";
        }
        public static bool HaveThisPrefabExist(string name)
        {
            foreach (var prefabName in ExistPrefabNames)
            {
                if (prefabName == name)
                {
                    return true;
                }
            }
            return false;
        }
        public static void AddPrefabToPrefabList(string name)
        {
            if (!HaveThisPrefabExist(name))
            {
                ExistPrefabNames.Add(name);
            }
        }

    }
    
}