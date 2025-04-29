using System;
using System.IO;
using UnityEditor;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public static class LogUtil
    {
        private static bool hadWarnning;
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
            Log($"[警告] {message}");  // 直接调用 Log，不再递归
        }

        public static void LogError(Exception err)
        {
            Log($"[错误] {err}");  // 直接调用 Log，不再递归
            ShowErrorDialog("出现错误", err);
            throw err;
        }
        // 会抛出错误，不需要手动抛出
        public static void LogError(AutoUIException err)
        {
            Log($"[错误] {err}");  // 直接调用 Log，不再递归
            ShowErrorDialog("出现错误", err);
            throw err;
        }
        public static void ShowErrorDialog(string title, Exception err)
        {
            EditorUtility.DisplayDialog(title, err.ToString(), "确定");
        }
        public static void ShowErrorDialog(string title, AutoUIException err)
        {
            EditorUtility.DisplayDialog(title, err.ToString(), "确定");
        }
        // 最后程序结束时调用
        public static void Hint()
        {
            if (hadWarnning)
            {
                EditorUtility.DisplayDialog("生成成功", "存在不严重的问题,请去桌面日志查看情况", "确定");
            }
            else
            {
                EditorUtility.DisplayDialog("提示", "运行成功", "确认");
            }
        }
    }
}

