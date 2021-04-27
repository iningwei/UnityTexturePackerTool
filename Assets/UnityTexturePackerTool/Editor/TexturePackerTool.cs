using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;
namespace UnityTexturePackerTool
{
    public class TexturePackerTool : Editor
    {
        static TexturePackerConfig texturePackerConfig;
        [MenuItem("TexturePackerTool/Build")]
        public static void BuildSprite()
        {
            texturePackerConfig = Resources.Load<TexturePackerConfig>("TexturePackerConfig");
            string originRootDir = Application.dataPath.Replace("Assets", "") + texturePackerConfig.originTextureFoldersRoot;
            UnityEngine.Debug.Log("originRootDir:" + originRootDir);
            //get all SpriteConfig
            List<string> spriteConfigFiles = new List<string>();
            SearchSpriteConfigFile(originRootDir, spriteConfigFiles);

            //check whether exist duplicate spriteName
            if (CheckSpriteNameDuplicate(spriteConfigFiles))
            {
                UnityEngine.Debug.LogError("error, exist duplicate spriteName, stop build");
                return;
            }

            //build texturepacker tmp file
            //build final spritetexture
            for (int i = 0; i < spriteConfigFiles.Count; i++)
            {

                string fileAssetsPath = spriteConfigFiles[i].Replace(Application.dataPath, "Assets");
                var spriteConfig = AssetDatabase.LoadAssetAtPath<SpriteConfig>(fileAssetsPath);

                BuildTexturePackerTmpFile(spriteConfig);
                BuildFinalSpriteTexture(spriteConfig);
            }

            AssetDatabase.Refresh();
        }

        static void SearchSpriteConfigFile(string path, in List<string> spriteConfigFiles)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();
            foreach (var f in files)
            {
                if (f.Name == "SpriteConfig.asset")
                {
                    string fileFullName = f.FullName.Replace("\\", "/");
                    spriteConfigFiles.Add(fileFullName);
                    UnityEngine.Debug.Log("add SpriteConfig.asset----->" + fileFullName);
                }
            }

            DirectoryInfo[] subDir = dir.GetDirectories();
            for (int i = 0; i < subDir.Length; i++)
            {

                SearchSpriteConfigFile(subDir[i].FullName, spriteConfigFiles);
            }
        }

        static bool CheckSpriteNameDuplicate(List<string> files)
        {
            List<string> tmpList = new List<string>();
            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i].Replace(Application.dataPath, "Assets");
                var config = AssetDatabase.LoadAssetAtPath<SpriteConfig>(file);
                UnityEngine.Debug.Log("checkSpriteName:" + config.spriteName);
                if (tmpList.Contains(config.spriteName))
                {
                    return true;
                }
                tmpList.Add(config.spriteName);
            }

            return false;
        }


        static void BuildTexturePackerTmpFile(SpriteConfig spriteConfig)
        {
            string commandText = " --sheet {0}.png --data {1}.xml --format sparrow --trim-mode None --pack-mode Best  --algorithm MaxRects --width {2} --height {3} --max-size {4} --size-constraints POT  --disable-rotation --scale 1 {5}";
            string inputPath = Application.dataPath.Replace("Assets", "") + spriteConfig.folderPath;

            StringBuilder sb = new StringBuilder("");
            string[] fileName = Directory.GetFiles(inputPath);
            for (int j = 0; j < fileName.Length; j++)
            {
                string extenstion = Path.GetExtension(fileName[j]);
                //TODO:format restrict
                if (extenstion == ".png")
                {
                    sb.Append(fileName[j]);
                    sb.Append("  ");
                }
                UnityEngine.Debug.Log($"fileName [{j}]:" + fileName[j]);
            }


            string tmpTexturePackOutputFile = Application.dataPath.Replace("Assets", "") + texturePackerConfig.tmpTexturePackerFileOutputFolder + "/" + spriteConfig.spriteName;

            processCommand("C:\\Program Files (x86)\\CodeAndWeb\\TexturePacker\\bin\\TexturePacker.exe",
                string.Format(commandText, tmpTexturePackOutputFile, tmpTexturePackOutputFile, spriteConfig.width, spriteConfig.height, spriteConfig.maxSize, sb.ToString()));

            AssetDatabase.Refresh();

        }

        static void BuildFinalSpriteTexture(SpriteConfig spriteConfig)
        {
            string tmpTexturePackerImgFile = Application.dataPath.Replace("Assets", "") + texturePackerConfig.tmpTexturePackerFileOutputFolder + "/" + spriteConfig.spriteName + ".png";
            string tmpTexturePackerXMLFile = Application.dataPath.Replace("Assets", "") + texturePackerConfig.tmpTexturePackerFileOutputFolder + "/" + spriteConfig.spriteName + ".xml";

            string finalSpriteTextureOutputFolder = Application.dataPath.Replace("Assets", "") + texturePackerConfig.spriteTextureOutputFolder;
            string finalSpriteTextureFullPath = finalSpriteTextureOutputFolder + "/" + spriteConfig.spriteName + ".png";

            if (File.Exists(finalSpriteTextureFullPath))
            {
                UnityEngine.Debug.Log("exist the same, delete:" + finalSpriteTextureFullPath);
                File.Delete(finalSpriteTextureFullPath);
                AssetDatabase.Refresh();//must refresh,otherwise file do not refresh
            }
            File.Copy(tmpTexturePackerImgFile, finalSpriteTextureFullPath);
            AssetDatabase.Refresh();

            FileStream fs = new FileStream(tmpTexturePackerXMLFile, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string jText = sr.ReadToEnd();
            fs.Close();
            sr.Close();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(jText);
            XmlNodeList elemList = xml.GetElementsByTagName("SubTexture");
            WriteMeta(elemList, spriteConfig.spriteName, spriteConfig);

            AssetDatabase.Refresh();
        }

        static string GetAssetPath(string path)
        {
            string[] seperator = { "Assets" };
            string p = "Assets" + path.Split(seperator, StringSplitOptions.RemoveEmptyEntries)[1];
            return p;
        }
        static void WriteMeta(XmlNodeList elemList, string sheetName, SpriteConfig spriteConfig)
        {
            string path = texturePackerConfig.spriteTextureOutputFolder + "/" + sheetName + ".png";
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            string impPath = AssetDatabase.GetAssetPath(texture);
            TextureImporter asetImp = TextureImporter.GetAtPath(impPath) as TextureImporter;
            SpriteMetaData[] metaData = new SpriteMetaData[elemList.Count];
            for (int i = 0, size = elemList.Count; i < size; i++)
            {
                XmlElement node = (XmlElement)elemList.Item(i);
                Rect rect = new Rect();
                rect.x = int.Parse(node.GetAttribute("x"));
                rect.y = texture.height - int.Parse(node.GetAttribute("y")) - int.Parse(node.GetAttribute("height"));
                rect.width = int.Parse(node.GetAttribute("width"));
                rect.height = int.Parse(node.GetAttribute("height"));
                metaData[i].rect = rect;
                metaData[i].pivot = new Vector2(0.5f, 0.5f);
                metaData[i].name = node.GetAttribute("name");

                //read source image's meta file,get spriteBorder msg, then write it to sprite
                string sourceImgFile = Application.dataPath.Replace("Assets", "") + spriteConfig.folderPath + "/" + node.GetAttribute("name") + ".png";

                Vector4 sourceBorder = GetTextureIpter(sourceImgFile).spriteBorder;
                UnityEngine.Debug.Log("image:" + sourceImgFile + ", border:" + sourceBorder.ToString());
                metaData[i].border = sourceBorder;

            }
            asetImp.spritesheet = metaData;
            asetImp.textureType = TextureImporterType.Sprite;
            asetImp.spriteImportMode = SpriteImportMode.Multiple;
            asetImp.mipmapEnabled = false;
            asetImp.SaveAndReimport();
        }

        static TextureImporter GetTextureIpter(string path)
        {
            TextureImporter textureIpter = null;
            Texture2D textureOrg = AssetDatabase.LoadAssetAtPath<Texture2D>(GetAssetPath(path));
            string impPath = AssetDatabase.GetAssetPath(textureOrg);
            textureIpter = TextureImporter.GetAtPath(impPath) as TextureImporter;
            return textureIpter;
        }

        private static void processCommand(string command, string argument)
        {
            ProcessStartInfo start = new ProcessStartInfo(command);
            start.Arguments = argument;
            start.CreateNoWindow = false;
            start.ErrorDialog = true;
            start.UseShellExecute = false;

            if (start.UseShellExecute)
            {
                start.RedirectStandardOutput = false;
                start.RedirectStandardError = false;
                start.RedirectStandardInput = false;
            }
            else
            {
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.RedirectStandardInput = true;
                start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
                start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
            }

            Process p = Process.Start(start);
            if (!start.UseShellExecute)
            {
                UnityEngine.Debug.Log(p.StandardOutput.ReadToEnd());
                UnityEngine.Debug.Log(p.StandardError.ReadToEnd());
            }

            p.WaitForExit();
            p.Close();
        }
    }
}