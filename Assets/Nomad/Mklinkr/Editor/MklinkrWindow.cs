using System.IO;
using UnityEngine;
using UnityEditor;

public class MklinkrWindow : EditorWindow
{
    public string Target;
    public string Source;

    [MenuItem("Nomad/Window/Mklinkr")]
    [MenuItem("Window/Nomad/Mklinkr")]
    private static void Open()
    {
        GetWindow<MklinkrWindow>();
    }

    private void OnGUI()
    {
        const int buttonWidth = 24;
        EditorStyles.textField.wordWrap = true;

        GUILayout.BeginVertical();
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Label("Target");
                GUILayout.BeginHorizontal();
                Target = EditorGUILayout.TextArea(Target);
                if (GUILayout.Button("...", GUILayout.Width(buttonWidth)))
                {
                    Target = EditorUtility.OpenFolderPanel("Select mklink target", "", "");
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            {
                GUILayout.Label("Source");
                GUILayout.BeginHorizontal();
                Source = EditorGUILayout.TextArea(Source);
                if (GUILayout.Button("...", GUILayout.Width(buttonWidth)))
                {
                    Source = EditorUtility.OpenFolderPanel("Select mklink source", "", "");
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            var targetIsNewOrEmpty = Mklinkr.ValidateIsNewOrEmptyDirectory(Target);
            var sourceOk = Directory.Exists(Source);

            // Info
            if (!targetIsNewOrEmpty)
            {
                EditorGUILayout.HelpBox("Target is not empty. It will be overwritten!", MessageType.Warning);
            }
            if (!sourceOk)
            {
                EditorGUILayout.HelpBox("Source directory is invalid", MessageType.Error );
            }

            // Validate
            if (!sourceOk)
            {
                GUI.enabled = false;
            }

            // Button
            if (GUILayout.Button("Make symbolic link", GUILayout.Height(34)))
            {
                Mklinkr.MakeLink(Target, Source);
            }
        }
        GUILayout.EndVertical();
    }
}