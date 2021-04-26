#if UNITY_EDITOR
using System;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class CommandBuild : Editor
{

    [MenuItem("Tools/SpritesPacker/CommandBuild")]
    public static void BuildTexturePacker()
    {
        //选择并设置TP命令行的参数和参数值(包括强制生成2048*2048的图集 --width 2048 --height 2048)
        string commandText = " --sheet {0}.png --data {1}.xml --format sparrow --trim-mode None --pack-mode Best  --algorithm MaxRects --width 512 --height 512 --max-size 1024 --size-constraints POT  --disable-rotation --scale 1 {2}";

        string inputPath = string.Format("{0}/Images", Application.dataPath);//小图目录
        string outputPath = string.Format("{0}/TexturePacker", Application.dataPath);//用TP打包好的图集存放目录
        string[] imagePath = Directory.GetDirectories(inputPath);

        for (int i = 0; i < imagePath.Length; i++)
        {
            UnityEngine.Debug.Log(imagePath[i]);
            StringBuilder sb = new StringBuilder("");
            string[] fileName = Directory.GetFiles(imagePath[i]);
            for (int j = 0; j < fileName.Length; j++)
            {
                string extenstion = Path.GetExtension(fileName[j]);
                if (extenstion == ".png")
                {
                    sb.Append(fileName[j]);
                    sb.Append("  ");


                }
                UnityEngine.Debug.Log("fileName [j]:" + fileName[j]);
            }
            string name = Path.GetFileName(imagePath[i]);
            string outputName = string.Format("{0}/TexturePacker/{1}/{2}", Application.dataPath, name, name);
            string sheetName = string.Format("{0}/SheetsByTP/{1}", Application.dataPath, name);
            //执行命令行
            processCommand("C:\\Program Files (x86)\\CodeAndWeb\\TexturePacker\\bin\\TexturePacker.exe", string.Format(commandText, sheetName, sheetName, sb.ToString()));
        }
        AssetDatabase.Refresh();
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
#endif
