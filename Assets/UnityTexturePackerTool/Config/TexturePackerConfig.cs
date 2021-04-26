using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace UnityTexturePackerTool
{
    [CreateAssetMenu(fileName = "TexturePackerConfig", menuName = "TexturePacker/TexturePackerConfig")]
    public class TexturePackerConfig : ScriptableObject
    {
        public string originTextureFoldersRoot;
        public string tmpTexturePackerFileOutputFolder;
        public string spriteTextureOutputFolder;



        private void OnValidate()
        {
            if (originTextureFoldersRoot.StartsWith("Assets") == false)
            {
                Debug.LogError("error, originTextureFoldersRoot not start with Assets");
            }
            if (tmpTexturePackerFileOutputFolder.StartsWith("Assets") == false)
            {
                Debug.LogError("error, tmpTexturePackerFileOutputFolder not start with Assets");
            }
            if (spriteTextureOutputFolder.StartsWith("Assets") == false)
            {
                Debug.LogError("error, spriteTextureOutputFolder not start with Assets");
            }

            string dirOriginTextureFoldersRoot = Application.dataPath.Replace("Asset", "") + originTextureFoldersRoot;
            if (Directory.Exists(dirOriginTextureFoldersRoot) == false)
            {
                Directory.CreateDirectory(dirOriginTextureFoldersRoot);
            }

            string dirTmpTexturePackerFileOutputFolder = Application.dataPath.Replace("Asset", "") + tmpTexturePackerFileOutputFolder;
            if (Directory.Exists(dirTmpTexturePackerFileOutputFolder) == false)
            {
                Directory.CreateDirectory(dirTmpTexturePackerFileOutputFolder);
            }

            string dirSpriteTextureOutputFolder = Application.dataPath.Replace("Asset", "") + spriteTextureOutputFolder;
            if (Directory.Exists(dirSpriteTextureOutputFolder) == false)
            {
                Directory.CreateDirectory(dirSpriteTextureOutputFolder);
            }
        }

    }
}