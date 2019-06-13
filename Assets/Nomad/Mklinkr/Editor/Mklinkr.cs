using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

public static class Mklinkr
{
    // /C : run and terminate
    // /K : run and remain
    public static void MakeLink(string targetDir, string sourceDir)
    {
        var cmdText = "";
        Process process;

        // Remove pre-existing directory
        if (Directory.Exists(targetDir))
        {
            cmdText = string.Format("/c rmdir /s/q \"{0}\"", targetDir);
            process = Process.Start("cmd.exe", cmdText);
            process.WaitForExit();

            //Directory.Delete(targetDir);
            Debug.LogFormat("[Mklinkr] Deleted directory at target ({0}).", targetDir);
        }

        cmdText = string.Format("/C mklink /j \"{0}\" \"{1}\"", targetDir, sourceDir);
        process = Process.Start("cmd.exe", cmdText);
        process.WaitForExit();

        if (Directory.Exists(sourceDir))
        {
            cmdText = string.Format("/select,\"{0}\"", targetDir.Replace(@"/", @"\")); // explorer doesn't like front slashes
            Process.Start("explorer.exe", cmdText);
            Debug.LogFormat("[Mklinkr] Created symbolic link at {0}", targetDir);
        }
        else
        {
            Debug.LogFormat("[Mklinkr] mklink failed");
        }
    }

    public static bool ValidateIsNewOrEmptyDirectory(string path)
    {
        if (!Directory.Exists(path))
            return true;

        var dirs = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);
        return dirs.Length + files.Length == 0;
    }
}