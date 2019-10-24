using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class MyBuildProcess {
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathAToBuiltProject)
    {
        Debug.Log(pathAToBuiltProject);
        string directory = Path.GetDirectoryName(pathAToBuiltProject);
        string fileName = Path.GetFileName(pathAToBuiltProject);

        switch (target)
        {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                File.WriteAllText(Path.Combine(directory, "StartServer.bat"), "start \"\"  \""+fileName+"\" -batchmode -nographics");
                break;
            case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal:
                break;
            case BuildTarget.StandaloneOSX:
                break;
            default:
                break;
        }
    }
}
