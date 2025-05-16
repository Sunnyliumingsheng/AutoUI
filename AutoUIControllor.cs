using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    // 这是一个对json数据进行递归处理和负责中断的线程。可以说是核心逻辑

    public class AutoUIControllor
    {

        // 原来你才是最终boss？！
        public static void MainControllor(GameObject canvasObj)
        {
            // 开始进行遍历
            RecursiveProcess(canvasObj);
        }
        private static void RecursiveProcess(GameObject parent)
        {
            if (checkExit()) return;
            Transform parentTransform = getTransform(parent);
            int count = getChildCount(parent);
            for (int i = 0; i < count; i++)
            {
                GameObject childGameObject = getChild(parent, i);
                Transform childTransform = getTransform(childGameObject);
                int childChildCount = getChildCount(childGameObject);
                if (childChildCount != 0)
                {
                    // 在PS中为Group意味着下面还有东西，并且由于group图层没有任何图片文字信息所以除了rectTransfrom之外不需要处理
                    // todo添加对组的处理
                    RecursiveProcess(childGameObject);
                    if (checkExit()) return;
                }
                else
                {
                    // 对应着PS中的图层，从这里开始就需要处理了
                    AutoUI.MainThread.Run(() =>
                    {
                        AutoUIUtil.FocusGameObject(childGameObject);
                    });

                }
            }
        }
        public static bool checkExit()
        {
            return !AutoUI.isRunning;
        }
        public static void exit()
        {
            AutoUI.isRunning = false;
        }
        /*
            用法解析
            检查子函数是否想要退出
            if(checkExit())return;
            自己想要退出
            exit();return;
        */
        public static Transform getTransform(GameObject go)
        {
            Transform transform = null;
            Task<Transform> task = AutoUI.MainThread.AsyncRun<Transform>(() => { return go.transform; });
            transform = task.Result;
            if (transform == null)
            {
                LogUtil.LogError("主线程获取transform失败");
                return null;
            }
            return transform;
        }
        public static int getChildCount(GameObject go)
        {
            int count = -1;
            var task=AutoUI.MainThread.AsyncRun<int>(() => { return go.transform.childCount; });
            count = task.Result;
            if (count == -1)
            {
                LogUtil.LogError("主线程获取子物体数量失败");
                return -1;
            }
            return count;
        }
        public static GameObject getChild(GameObject go, int index)
        {
            GameObject child = null;
            var task=AutoUI.MainThread.AsyncRun<GameObject>(() => { return go.transform.GetChild(index).gameObject; });
            child = task.Result; 
            if (child == null)
            {
                LogUtil.LogError("主线程获取子物体失败");
                return null;
            }
            return child;
        }

    }

}