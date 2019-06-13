using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName ="Nomad/Symlink")]
public class SymlinkAsset : ScriptableObject
{
    public string SourcePath;
    public string TargetPath;

    public void RebuildLink()
    {
        Mklinkr.MakeLink(TargetPath, SourcePath);
    }

    public static SymlinkAsset GetAsset(string name)
    {
        var guids = AssetDatabase.FindAssets("t:symlinkasset", new string[] { "Assets" });
        var paths = guids.Select(guid => AssetDatabase.GUIDToAssetPath(guid)).ToArray();
        foreach (var path in paths)
        {
            if (Path.GetFileNameWithoutExtension(path) == name)
            {

            }
        }
        return null;
    }
}