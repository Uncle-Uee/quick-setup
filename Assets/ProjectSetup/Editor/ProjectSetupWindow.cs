#if UNITY_EDITOR
using PackageCreator.Serializables;
using UnityEditor;
using UnityEngine;

namespace PackageCreator
{
    public class ProjectSetupWindow : EditorWindow
    {
        #region WINDOW

        private static ProjectSetupWindow _window;

        [MenuItem("Setup/Project Folders", false, 10)]
        public static void OpenWindow()
        {
            _window = GetWindow<ProjectSetupWindow>("Setup");
            _window.Show();
            _window.maxSize = _window.minSize = new Vector2(360f, 128f);
            
        }

        #endregion

        #region VARIABLES

        private TextAsset _custom;

        #endregion

        #region PROPERTIES

        private TextAsset _editorTool => Resources.Load<TextAsset>("Layouts/editor-tool");
        private TextAsset _runtimeApi => Resources.Load<TextAsset>("Layouts/runtime-api");
        private TextAsset _standard   => Resources.Load<TextAsset>("Layouts/standard");

        #endregion

        #region UNITY METHODS

        private void OnEnable()
        {
            _custom = Resources.Load<TextAsset>("Layouts/template");
        }

        public void OnGUI()
        {
            CustomPackageFileField();
            SetupOptionsButtons();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(this);
            }
        }

        #endregion

        #region METHODS

        private void CustomPackageFileField()
        {
            _custom =
                EditorGUILayout.ObjectField("Custom Layout File:", _custom, typeof(Object), false) as TextAsset;
            EditorGUILayout.Space();
        }

        private void SetupOptionsButtons()
        {
            if (GUILayout.Button("Editor Tool Layout"))
            {
                ProjectSetup(_editorTool.text);
            }

            if (GUILayout.Button("Runtime API Layout"))
            {
                ProjectSetup(_runtimeApi.text);
            }

            if (GUILayout.Button("Standard Layout"))
            {
                ProjectSetup(_standard.text);
            }

            if (GUILayout.Button("Custom Layout"))
            {
                ProjectSetup(_custom?.text);
            }
        }

        private void ProjectSetup(string data)
        {
            JsonUtility.FromJson<Folders>(data).CreateFolders();
            AssetDatabase.Refresh();
        }

        #endregion
    }
}

#endif