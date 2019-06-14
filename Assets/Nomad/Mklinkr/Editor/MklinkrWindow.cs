using System;
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

    private bool TryGetPathFromDrop(Rect rect, ref string path)
    {
        var evt = Event.current;
        GUI.enabled = true;

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:

                if (!rect.Contains(evt.mousePosition))
                {
                    return false;
                }

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    if (DragAndDrop.paths.Length > 0)
                    {
                        path = DragAndDrop.paths[0];
                        return true;
                    }
                }
                break;
        }
        return false;
    }

    private void OnGUI_MakeLink()
    {
        // Edit Path to Source 
        Rect sourceDropRect;
        sourceDropRect = EditorGUILayout.BeginVertical();
        {
            GUILayout.Label("Source:");
            EditorGUILayout.BeginHorizontal();
            {
                _pathToSource = EditorGUILayout.TextArea(_pathToSource);
                TryGetPathFromDrop(sourceDropRect, ref _pathToSource);
                if (GUILayout.Button("...", GUILayout.Width(_buttonWidth)))
                {
                    GUI.SetNextControlName("btn");
                    _pathToSource = EditorUtility.OpenFolderPanel("Select mklink source", "", "");
                    GUI.FocusControl("btn");
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();


        // Edit Path to Target 
        Rect targetDropRect;
        targetDropRect = EditorGUILayout.BeginVertical();
        {
            GUILayout.Label("Target:");
            EditorGUILayout.BeginHorizontal();
            {
                _pathToTarget = EditorGUILayout.TextArea(_pathToTarget);
                TryGetPathFromDrop(targetDropRect, ref _pathToTarget);
                if (GUILayout.Button("...", GUILayout.Width(_buttonWidth)))
                {
                    GUI.SetNextControlName("btn");
                    _pathToTarget = EditorUtility.OpenFolderPanel("Select mklink target", "", "");
                    GUI.FocusControl("btn");
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

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

    private void OnGUI_RemoveLink()
    {
        // Edit Path
        Rect dropRect;
        dropRect = EditorGUILayout.BeginVertical();
        {
            GUILayout.Label("Directory to Clean:");
            EditorGUILayout.BeginHorizontal();
            {
                _pathToClean = EditorGUILayout.TextArea(_pathToClean);
                if (GUILayout.Button("...", GUILayout.Width(_buttonWidth)))
                {
                    GUI.SetNextControlName("btn");
                    _pathToClean = EditorUtility.OpenFolderPanel("Select mklink target", "", "");
                    GUI.FocusControl("btn");
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        TryGetPathFromDrop(dropRect, ref _pathToClean);


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