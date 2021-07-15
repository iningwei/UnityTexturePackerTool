using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace UnityTexturePackerTool
{
    [CreateAssetMenu(fileName = "SpriteConfig", menuName = "TexturePacker/SpriteConfig")]
    public class SpriteConfig : ScriptableObject
    {
        public bool enableBuild= true;
        public string filePath;
        public string folderPath;
        public string spriteName;
        public int width = 512;
        public int height = 512;
        public int maxSize = 1024;
        [Header("pics will not be packed into atlas")]
        public List<string> exceptions;

        private void OnValidate()
        {
            int instanceId = this.GetInstanceID();
            this.filePath = AssetDatabase.GetAssetPath(instanceId);
            this.folderPath = new FileInfo(Application.dataPath.Replace("Assets", "") + this.filePath).Directory.FullName.Replace("\\", "/");
            this.folderPath = this.folderPath.Replace(Application.dataPath, "Assets");


            if (string.IsNullOrEmpty(this.spriteName.Trim()))
            {
                Debug.LogError("error spriteName is empty,please check:" + this.filePath);
            }
        }
    }
}
#endif