
using System;
using System.Drawing;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class AutoUIImagesImportProcessor : EditorWindow
    {
        // 将图片导入到unity
        public static void ImageImportProcessor(string selectedFolderPath)
        {

            const string extension = "*.png";
            string[] imageFiles = Directory.GetFiles(selectedFolderPath, extension);
            foreach (string imageFile in imageFiles)
            {
                string fileName = Path.GetFileName(imageFile);
                string destFilePath = ParseImageName(fileName);
                string srcImagePath = Path.Combine(selectedFolderPath, fileName);
                if (!IsImageExist(destFilePath))
                {
                    File.Copy(imageFile, destFilePath, true);
                }
                TextureImporter importer = AssetImporter.GetAtPath(destFilePath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    // todo ：判断是不是一个需要被九宫格切图的图片
                    // todo : 进行九宫格切图
                    AutoUI.imageNameToSpritePath.Add(fileName, destFilePath);
                    importer.SaveAndReimport();
                    AssetDatabase.Refresh();
                    LogUtil.Log("已经保存文件:" + destFilePath);
                }
            }
            AssetDatabase.Refresh();
        }

        public static bool IsImageExist(string filePath)
        {
            return File.Exists(filePath);
        }


        // 根据图片的名称进行分析，确定应该放到哪个图片文件夹中,返回的是一个文件路径，不需要用/开头
        // .png的问题，你输入没有，输出时就没有。输入时有，输出时就有
        public static string ParseImageName(string imageName)
        {
            return "Assets/Images/" + imageName;
        }

        // 导入一张图片 ，图片的路径和想要保存到哪个文件夹
        public static void ImportImage(string imagePath,string selectedFolderPath)
        {
            string fileName = Path.GetFileName(imagePath);
            string destFilePath = ParseImageName(fileName);
            string srcImagePath = Path.Combine(selectedFolderPath, fileName);
            if (!IsImageExist(destFilePath))
            {
                File.Copy(imagePath, destFilePath, true);
            }
            TextureImporter importer = AssetImporter.GetAtPath(destFilePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                AutoUI.imageNameToSpritePath.Add(fileName, destFilePath);
                importer.SaveAndReimport();
                AssetDatabase.Refresh();
                LogUtil.Log("已经保存文件:" + destFilePath);
            }
        }
        // todo添加一个九宫格切图的功能

    }




}