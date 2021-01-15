using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PackageCreator.Serializables
{
    [Serializable]
    public class Folders
    {
        #region VARIABLES

        public List<string> Paths = new List<string>();

        #endregion

        #region METHODS

        public void CreateFolders()
        {
            foreach (string path in Paths)
            {
                if (Directory.Exists(path)) continue;
                Debug.Log($"Created Folder: {path}");
                Directory.CreateDirectory(Path.Combine("Assets", path));
            }
        }

        #endregion
    }
}