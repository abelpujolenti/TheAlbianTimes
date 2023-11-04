using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileManager
{
    public static void LoadAllJsonFiles(string jsonPath, Action<string> addTo)
    {
        string path = Application.streamingAssetsPath + "/Json/" + jsonPath;
        if (Directory.Exists(path))
        {
            DirectoryInfo d = new DirectoryInfo(path);
            foreach (var file in d.GetFiles("*.json"))
            {
                using (StreamReader sr = file.OpenText())
                {
                    addTo(sr.ReadToEnd());
                }
            }
        }
    }
}
