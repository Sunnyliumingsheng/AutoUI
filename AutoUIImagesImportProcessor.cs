
using System;
using System.IO;
using UnityEditor;

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
            // todo : 写一个名称对应文件夹的匹配树
            
            /*
            这里由于mywar项目的历史遗留问题太重，根本无法做到命名很文件夹之间的一一对应。所以这里全部注释掉。如果以后美术愿意一个个修改名字，就可以
            */
            
            // string[] parts= imageName.Split('_');
            // if(AutoUIConfig.SpriteNameProject1.TryGetValue(parts[0],out string project1)){
            //     switch (project1){
            //         case "Sprites":
            //             if(AutoUIConfig.SpriteNameProject2SP.TryGetValue(parts[1],out string project2)){
            //                return AutoUIConfig.config.sprite.spritePath+"/"+project1+"/"+project2+"/"+imageName;
            //             }else{
            //                 var err= new AutoUIException("名称第二层解析错误请更新图片与名称的映射规则,输入为:"+imageName);
            //                 LogUtil.AddWarning(err);
            //             }
            //             break;
            //         case "HUD":
            //             return AutoUIConfig.config.sprite.spritePath+"/"+project1+"/"+imageName;
            //     }
            // }
            // else{
            //     var err= new AutoUIException("名称第一层解析错误请更新图片与名称的映射规则,输入为:"+imageName);
            //     LogUtil.AddWarning(err); 
            // }


            return "Assets/Images/" + imageName;
        }
    }




}