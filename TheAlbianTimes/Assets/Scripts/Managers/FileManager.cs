using System;
using System.IO;
using UnityEngine;

namespace Managers
{
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

        public static string LoadJsonFile(string jsonPath)
        {
            string path = Application.streamingAssetsPath + "/Json/" + jsonPath;
            return !File.Exists(path) ? "" : File.ReadAllText(Application.streamingAssetsPath + jsonPath);
        }
    }
}
