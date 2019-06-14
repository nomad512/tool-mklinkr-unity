using System.IO;
using UnityEngine;
using UnityEditor;

public class MklinkrWindow : EditorWindow
{
    private string _pathToTarget;
    private string _pathToSource;
    private string _pathToClean;

    private const int _buttonWidth = 24;
    private readonly string[] _toolbarTabs = new string[] { "Make", "Remove" };

    private int _toolbarTab = 0;

    [MenuItem("Nomad/Window/Mklinkr")]
    [MenuItem("Window/Nomad/Mklinkr")]
    private static void Open()
    {
        GetWindow<MklinkrWindow>();
    }

    private void OnGUI()
    {
        EditorStyles.textField.wordWrap = true;

        _toolbarTab = GUILayout.Toolbar(_toolbarTab, _toolbarTabs);
        switch (_toolbarTab)
        {
            default:
            case 0:
                OnGUI_MakeLink();
                break;

            case 1:
                OnGUI_RemoveLink();
                break;
        }
    }

    private void OnGUI_MakeLink()
    {
        // Edit Path to Source 
        GUILayout.BeginVertical();
        {
            GUILayout.Label("Source:");
            GUILayout.BeginHorizontal();
            _pathToSource = EditorGUILayout.TextArea(_pathToSource);
            if (GUILayout.Button("...", GUILayout.Width(_buttonWidth)))
            {
                GUI.SetNextControlName("btn");
                _pathToSource = EditorUtility.OpenFolderPanel("Select mklink source", "", "");
                GUI.FocusControl("btn");
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        // Edit Path to Target 
        GUILayout.BeginVertical();
        {
            GUILayout.Label("Target:");
            GUILayout.BeginHorizontal();
            _pathToTarget = EditorGUILayout.TextArea(_pathToTarget);
            if (GUILayout.Button("...", GUILayout.Width(_buttonWidth)))
            {
                GUI.SetNextControlName("btn");
                _pathToTarget = EditorUtility.OpenFolderPanel("Select mklink target", "", "");
                GUI.FocusControl("btn");
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        var targetIsNewOrEmpty = Mklinkr.ValidateIsNewOrEmptyDirectory(_pathToTarget);
        var sourceOk = Directory.Exists(_pathToSource);
        bool targetOk;
        try
        {
            targetOk = Path.IsPathRooted(_pathToTarget);
        }
        catch
        {
            targetOk = false;
        }

        // Display Info
        if (!targetIsNewOrEmpty)
        {
            EditorGUILayout.HelpBox("Target is not empty. It will be overwritten!", MessageType.Warning);
        }
        if (!sourceOk)
        {
            EditorGUILayout.HelpBox("Source directory is invalid", MessageType.Error);
        }
        if (!targetOk)
        {
            EditorGUILayout.HelpBox("Target directory is invalid", MessageType.Error);
        }
        if (sourceOk && targetOk)
        {
            EditorGUILayout.HelpBox("Ready to create a symbolic link :)", MessageType.Info);
        }

        // Button
        GUI.enabled = sourceOk && targetOk;
        if (GUILayout.Button("Make Symbolic Link", GUILayout.Height(34)))
        {
            Mklinkr.MakeLink(_pathToSource, _pathToTarget);
        }
    }

    string _cachePath;
    string _finalPath;

    private void OnGUI_RemoveLink()
    {
        // Edit Path
        GUILayout.BeginVertical();
        {
            GUILayout.Label("Directory to Clean:");
            GUILayout.BeginHorizontal();
            _pathToClean = EditorGUILayout.TextArea(_pathToClean);
            if (GUILayout.Button("...", GUILayout.Width(_buttonWidth)))
            {
                GUI.SetNextControlName("btn");
                _pathToClean = EditorUtility.OpenFolderPanel("Select mklink target", "", "");
                GUI.FocusControl("btn");
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        var directoryExists = Directory.Exists(_pathToClean);
        var isSymlink = false;

        // Info
        if (!directoryExists)
        {
            EditorGUILayout.HelpBox("The directory is invalid", MessageType.Error);
        }
        else
        {
            isSymlink = Mklinkr.IsSymbolic(_pathToClean);
            if (isSymlink)
            {
                EditorGUILayout.HelpBox("The directory is a Symbolic Link", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("The directory is not a Symbolic Link", MessageType.Error);
            }
        }

        // Button
        GUI.enabled = directoryExists && isSymlink;
        if (GUILayout.Button("Remove Symbolic Link", GUILayout.Height(34)))
        {
            Mklinkr.RemoveLink(_pathToClean);
        }
    }
}