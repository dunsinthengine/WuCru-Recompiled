#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

[InitializeOnLoad]
public static class BuildReleaseMenu
{
    static BuildReleaseMenu()
    {
        EditorApplication.update += CheckAutoBuild;
    }

    private static void CheckAutoBuild()
    {
        string triggerPath = Path.Combine(Application.dataPath, "..", "build_trigger.txt");
        if (File.Exists(triggerPath))
        {
            try
            {
                File.Delete(triggerPath);
            }
            catch {}
            BuildStandaloneWindows();
        }
    }

    [MenuItem("Tools/Build Windows Standalone (Release)")]
    public static void BuildStandaloneWindows()
    {
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", ".."));
        string buildDir = Path.Combine(projectRoot, "Build_Windows");
        string exePath = Path.Combine(buildDir, "WuCru.exe");

        if (Directory.Exists(buildDir))
        {
            try
            {
                Directory.Delete(buildDir, true);
            }
            catch {}
        }
        Directory.CreateDirectory(buildDir);

        string[] scenes = new string[]
        {
            "Assets/Scenes/Amuzo/Main.unity",
            "Assets/Scenes/Amuzo/Loading.unity",
            "Assets/Scenes/Amuzo/Debug/LegoSdkInit.unity",
            "Assets/Scenes/LevelPlayerNew.unity",
            "Assets/Scenes/Amuzo/Empty.unity",
            "Assets/Scenes/Amuzo/Dojo.unity"
        };

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = exePath;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        Debug.Log("[BuildScript] Starting Windows Standalone build to: " + exePath);
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        string statusFile = Path.Combine(projectRoot, "build_status.txt");
        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("[BuildScript] Build SUCCEEDED: " + summary.totalSize + " bytes in " + summary.totalTime.TotalSeconds + "s");
            File.WriteAllText(statusFile, "SUCCESS: " + exePath);
        }
        else
        {
            Debug.LogError("[BuildScript] Build FAILED with " + summary.totalErrors + " errors.");
            File.WriteAllText(statusFile, "FAILED: " + summary.totalErrors + " errors");
        }
    }
}
#endif
