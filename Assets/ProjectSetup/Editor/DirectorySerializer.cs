#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JellyFish.Setup
{
    public static class DirectorySerializer
    {
        #region MAGIC METHOD

        /// <summary>
        /// Deserialize Json Dialog Panel Options.
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        [OnOpenAsset(0)]
        public static bool DeserializeQSJsonMagicMethod(int instanceId, int line)
        {
            Object _object = EditorUtility.InstanceIDToObject(instanceId);

            string path = AssetDatabase.GetAssetPath(_object);

            if (Path.GetExtension(path)?.ToLower() == ".pdjson")
            {
                // Creates Dialog
                int doThis = EditorUtility.DisplayDialogComplex("Create Project Directories",
                                                                "Do you want to create the Projects Directories?",
                                                                "Create Directories", "Cancel", "Open File");

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
        [MenuItem("Assets/JellyFish/Setup/Project/Serialize Folders", priority = 10)]
        public static void Serialize()
        {
            try
            {
                // Allow Multiple Objects to be Selected.
                Object[] objects = Selection.objects;

                string settingsFile = "";
                DirectoryStructure directoryStructure = new DirectoryStructure();

                foreach (Object _object in objects)
                {
                    if (string.IsNullOrEmpty(settingsFile))
                    {
                        settingsFile =
                            EditorUtility.SaveFilePanel("Save Project Directory Structure", Application.dataPath,
                                                        $"project-directory-structure", "pdjson");
                    }

                    string objectPath = AssetDatabase.GetAssetPath(_object);

                    if (Directory.Exists(objectPath))
                    {
                        if (!directoryStructure.Directories.Contains(objectPath))
                        {
                            directoryStructure.Directories.Add(objectPath);
                        }
                        
                        // Get All Folders and Subfolders.
                        string[] paths = Directory.GetDirectories(objectPath, "*", SearchOption.AllDirectories);
                        directoryStructure.Directories.AddRange(paths);
                    }
                    else
                    {
                        Debug.LogWarning("You have not selected a Folder!");
                    }
                }

                if (directoryStructure.Directories.Count > 0)
                {
                    string json = JsonUtility.ToJson(directoryStructure, true);
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
        /// Deserialize Project Setup Settings File to a Valid Project Folder Layout.
        /// </summary>
        [MenuItem("Assets/JellyFish//Setup/Project/Deserialize PDJson", priority = 10)]
        public static void Deserialize()
        {
            try
            {
                Object _object = Selection.activeObject;

                if (_object)
                {
                    string path = AssetDatabase.GetAssetPath(_object);

                    if (Path.GetExtension(path)?.ToLower() == ".pdjson")
                    {
                        CreateDirectories(path);
                    }
                    else
                    {
                        path = EditorUtility.OpenFilePanel("Select PDJson File", Application.dataPath, "pdjson");
                        CreateDirectories(path);
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.Log(exception.ToString());
            }
        }

        /// <summary>
        /// Create Directories from a file at a path.
        /// </summary>
        /// <param name="path"></param>
        private static void CreateDirectories(string path)
        {
            string json = File.ReadAllText(path);
            DirectoryStructure directoryStructure = JsonUtility.FromJson<DirectoryStructure>(json);

            directoryStructure.Directories.ForEach(directory =>
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Debug.Log($"Created Directory: {directory}");
                }
            });
            AssetDatabase.Refresh();
        }

        #endregion
    }
}
#endif