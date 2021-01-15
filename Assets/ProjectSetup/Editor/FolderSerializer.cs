#if UNITY_EDITOR
using System;
using System.IO;
using PackageCreator.Serializables;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PackageCreator
{
    public static class FolderSerializer
    {
        #region MAGIC METHOD

        /// <summary>
        /// Deserialize Json Dialog Panel Options.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        [OnOpenAsset(0)]
        public static bool JsonMagicMethod(int instanceId, int line)
        {
            Object _object = EditorUtility.InstanceIDToObject(instanceId);

            string path = AssetDatabase.GetAssetPath(_object);

            if (Path.GetExtension(path)?.ToLower() == ".json")
            {
                // Creates Dialog
                int doThis = EditorUtility.DisplayDialogComplex("Create Project Directories",
                                                                "Is this a valid file?\n[Json File containing Folder Paths]",
                                                                "Yes, Create Directories", "Cancel", "Open File");

                if (doThis == 0)
                {
                    CreateDirectories(path);

                    return true;
                }

                if (doThis == 1)
                {
                    return true;
                }

                if (doThis == 2)
                {
                    return false;
                }
            }

            return false;
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Serialize Folder Layout to a Valid Project Setup Json File.
        /// </summary>
        [MenuItem("Assets/Folders/Folders To Json", priority = 10)]
        public static void Serialize()
        {
            try
            {
                // Allow Multiple Objects to be Selected.
                Object[] objects = Selection.objects;

                string settingsFile = "";
                Folders folders = new Folders();

                foreach (Object _object in objects)
                {
                    if (string.IsNullOrEmpty(settingsFile))
                    {
                        settingsFile =
                            EditorUtility.SaveFilePanel("Save Project Directory Structure", Application.dataPath,
                                                        $"project-directory-structure", "json");
                    }

                    string objectPath = AssetDatabase.GetAssetPath(_object);

                    if (Directory.Exists(objectPath))
                    {
                        if (!folders.Paths.Contains(objectPath))
                        {
                            folders.Paths.Add(objectPath);
                        }

                        // Get All Folders and Subfolders.
                        string[] paths = Directory.GetDirectories(objectPath, "*", SearchOption.AllDirectories);
                        folders.Paths.AddRange(paths);
                    }
                    else
                    {
                        Debug.LogWarning("You have not selected a Folder!");
                    }
                }

                if (folders.Paths.Count > 0)
                {
                    string json = JsonUtility.ToJson(folders, true);
                    File.WriteAllText(settingsFile, json);
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.Log("There are no Valid Directories to Serialize.");
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.ToString());
            }
        }

        /// <summary>
        /// Create Directories from a file at a path.
        /// </summary>
        /// <param name="path"></param>
        private static void CreateDirectories(string path)
        {
            JsonUtility.FromJson<Folders>(File.ReadAllText(path)).CreateFolders();
            AssetDatabase.Refresh();
        }

        #endregion
    }
}
#endif