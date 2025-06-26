using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace AutoUI
{
    public static class LogUtil
    {
        private static bool hadWarnning;
        private static List<string> LogWarningList = new List<string>();
        private static string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "unity_ui_log.txt");
        private static bool isFirstWrite = true;
        public static void ClearLogFile()
        {
            try
            {
                // 清空文件内容
                File.WriteAllText(logPath, string.Empty);
                isFirstWrite = true; // 重置标志位，下次写入时会重新写入标题
                Console.WriteLine("日志文件已清空");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[日志系统错误] 无法清空日志文件: {e.Message}");
            }
        }
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
            hadWarnning = true;
            LogWarningList.Add(message);
            Log($"[警告] {message}");
        }

        public static void LogError(string message)
        {
            throw new System.Exception(message); ;
        }
        public static void ShowErrorDialog(string title, string errMessage)
        {
            EditorUtility.DisplayDialog(title, errMessage, "确定");
        }
        public static void HandleAutoUIError(Exception err)
        {
            ShowErrorDialog("出现错误", err.Message);
            Log($"[错误] {err.Message} ");
            Log("stackTrace:" + err.StackTrace);
            Hint();
        }

        // 最后程序结束时调用
        public static void Hint()
        {
            AutoUIGroupLayerProcessor.ClearExistPrefabNames();
            if (LogWarningList.Count > 0)
            {
                Log("=== 警告日志 ===");
                foreach (string warning in LogWarningList)
                {
                    Log("警告汇总" + warning);
                }
            }
            if (hadWarnning)
            {
                EditorUtility.DisplayDialog("生成成功", "存在不严重的问题,请去桌面日志查看情况\n 提示: 请检索[警告]", "确定");
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "运行成功", "确认");
            }
        }
    }
}

