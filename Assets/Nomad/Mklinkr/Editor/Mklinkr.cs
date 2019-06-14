using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Debug = UnityEngine.Debug;
using UnityEditor;

static public class Mklinkr
{
    /// <summary>
    /// 
    /// </summary>
    static public void MakeLink(string sourceDir, string targetDir)
    {
        var cmdText = "";
        Process process;
        SanitaizeForExplorer(ref sourceDir);
        SanitaizeForExplorer(ref targetDir);

        // Remove pre-existing directory
        if (Directory.Exists(targetDir))
        {
            cmdText = string.Format("/C rmdir /s/q \"{0}\"", targetDir); // "/C" = run and terminate ... "/K" = run and remain
            process = Process.Start("cmd.exe", cmdText);
            process.WaitForExit();

            //Directory.Delete(targetDir);
            Debug.LogFormat("[Mklinkr] Overwriting directory at target... ({0})", targetDir);
        }

        cmdText = string.Format("/C mklink /j \"{0}\" \"{1}\"", targetDir, sourceDir);
        process = Process.Start("cmd.exe", cmdText);
        process.WaitForExit();

        if (Directory.Exists(sourceDir))
        {
            cmdText = string.Format("/select,\"{0}\"", targetDir); 
            Process.Start("explorer.exe", cmdText);
            Debug.LogFormat("[Mklinkr] Created symbolic link at {0}.", targetDir);

            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogFormat("[Mklinkr] mklink failed");
        }
    }

    /// <summary>
    /// Remove a symbolic link at the path but replace with the copied contents in the same location.
    /// </summary>
    static public void RemoveLink(string path)
    {
        var cmdText = "";
        Process process;

        SanitaizeForExplorer(ref path);

        var tempDirPath = path + "_temp";
        var dirName = Path.GetFileName(path);
        var tempDirName = Path.GetFileName(tempDirPath);

        // Because the files do not exist in this location (they are symbolically linked) they must first be copied to a new directory.
        cmdText = string.Format("/C xcopy \"{0}\" \"{1}\" /s/i", path, tempDirPath);
        process = Process.Start("cmd.exe", cmdText);
        process.WaitForExit();

        // With the directory copied, delete the symlink directory (without deleting the symlink source).
        RemoveDirectory(path);

        // Rename copied directory from temp name to original name.
        cmdText = string.Format("/C ren \"{0}\" \"{1}\"", tempDirPath, dirName);
        process = Process.Start("cmd.exe", cmdText);
        process.WaitForExit();

        AssetDatabase.Refresh();

        if (Directory.Exists(path))
        {
            Debug.Log("[Mklinkr] Symlink was removed.");
        }
    }

    static public void RemoveDirectory(string path)
    {
        var cmdText = "";
        Process process;

        SanitaizeForExplorer(ref path);

        // Remove pre-existing directory
        if (Directory.Exists(path))
        {
            cmdText = string.Format("/c rmdir /s/q \"{0}\"", path);
            process = Process.Start("cmd.exe", cmdText);
            process.WaitForExit();
            Debug.LogFormat("[Mklinkr] Removed directory at {0}.", path);
        }
    }

    static public bool IsSymbolic(string path)
    {
        FileInfo pathInfo = new FileInfo(path);

        var variable = pathInfo.Attributes;
        var flag = FileAttributes.ReparsePoint;

        if (!Enum.IsDefined(variable.GetType(), flag))
        {
            throw new ArgumentException(string.Format(
                "Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.",
                flag.GetType(), variable.GetType()));
        }

        ulong num = Convert.ToUInt64(flag);
        return ((Convert.ToUInt64(variable) & num) == num);
    }

    static public bool ValidateIsNewOrEmptyDirectory(string path)
    {
        if (!Directory.Exists(path))
            return true;

        var dirs = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);
        return dirs.Length + files.Length == 0;
    }


    static private string SanitaizeForExplorer(ref string path)
    {
        path = path.Replace(@"/", @"\"); // explorer doesn't like front slashes
        return path;
    }

}