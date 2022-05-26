using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

public class GameBuildPipeline
{
    private static readonly string TARGET_DIR = "/Build";

    private static string buildDir = TARGET_DIR;

    public enum StagingEnvironment
    {
        DEV,
        PROD
    }

    public static StagingEnvironment CurrentStagingEnvironment = StagingEnvironment.DEV;
    public static readonly string ENVIRONMENT_DEFINE_SUFFIX = "_ENVIRONMENT";

    static List<string> _baseScenes = new List<string>();


    [MenuItem("Builds/Build Game")]
    public static void PerformBuildForActivePlatform()
    {
        ParseCommandLineParameters();
        ExecuteBuild();
    }

    public static void SetEnvironmentDefinesForPlatform(BuildTarget buildTarget)
    {
        BuildTargetGroup targetGroup = BuildTargetGroup.WebGL;
        List<string> defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';').ToList();

        // Remove previous environment defines
        defines.Remove(GetEnvironmentDefineForStage(StagingEnvironment.DEV));
        defines.Remove(GetEnvironmentDefineForStage(StagingEnvironment.PROD));

        // Only include defines once
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines.Distinct().ToArray());
    }

    static string GetEnvironmentDefineForStage(StagingEnvironment stage)
    {
        return $"{stage}{ENVIRONMENT_DEFINE_SUFFIX}";
    }

    public static void ExecuteBuild(bool exitOnSuccess = true)
    {
        PreprocessBuild();

        string targetDir = Path.Combine(Application.dataPath, "../Build");
        string targetPath = targetDir;

        //clean up existing output path first
        if (Directory.Exists(targetPath))
        {
            Directory.Delete(targetPath, true);
        }

        Directory.CreateDirectory(targetPath);

        BuildOptions buildOptions = BuildOptions.None;

        AssetDatabase.Refresh();
        BuildReport report = BuildPipeline.BuildPlayer(_baseScenes.ToArray(), targetDir, EditorUserBuildSettings.activeBuildTarget, buildOptions);
        BuildSummary summary = report.summary;

        // copy build data to frontend directory
        CopyBuiltData();

        if (summary.result == BuildResult.Succeeded)
        {
            if (!exitOnSuccess)
            {
                return;
            }

            EditorUtils.FinishBuild(EditorUtils.BuildErrorType.NoError);
        }
        else
        {
            throw new Exception("BuildPlayer failure: " + summary.result);
        }
    }
    
    [MenuItem("Builds/Substeps/Copy Built Data")]
    private static void CopyBuiltData()
    {
        string srcPath = Path.Combine(Application.dataPath, "../Build/Build");
        string targetFilePath = Path.Combine(Application.dataPath, "../../solaniverse-viewer/public/Build");

        try
        {
            if (!Directory.Exists(targetFilePath))
            {
                Directory.CreateDirectory(targetFilePath);
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(srcPath, targetFilePath), true);
            }

            Debug.Log("Built data copied from: " + srcPath + " to: " + targetFilePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error copying built data: " + e.ToString());
            EditorUtils.FinishBuild(EditorUtils.BuildErrorType.BuiltDataCopyFail);
        }
    }

    public static void GetScenesForBuild(List<string> baseScenes)
    {
        baseScenes.Clear();

        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene == null || !scene.enabled || string.IsNullOrEmpty(scene.path))
            {
                continue;
            }

            baseScenes.Add(scene.path);
        }
    }


    // common steps
    [MenuItem("Builds/Substeps/Preprocess build")]
    static void PreprocessBuild()
    {
        GetScenesForBuild(_baseScenes);
        SetEnvironmentDefinesForPlatform(EditorUserBuildSettings.activeBuildTarget);
    }

    public static void ParseCommandLineParameters()
    {
        string stagingEnvironment = string.Empty;
        string gitRevision = string.Empty;
        int buildNumber = 0;

        for (int i = 0; i < Environment.GetCommandLineArgs().Length; i++)
        {
            string param = Environment.GetCommandLineArgs()[i];
            if (string.IsNullOrEmpty(param))
            {
                continue;
            }

            if (param[0] == '-')
            {
                string paramName = param.Substring(1, param.Length - 1);
                string paramValue = Environment.GetCommandLineArgs().Length > i + 1 ? Environment.GetCommandLineArgs()[i + 1] : "";

                if (paramName == "buildNumber")
                {
                    int.TryParse(paramValue, out buildNumber);
                }
                else if (paramName == "gitRevision")
                {
                    gitRevision = paramValue;
                }
                else if (paramName == "stagingEnvironment")
                {
                    stagingEnvironment = paramValue;
                }
                else if (paramName == "buildDir")
                {
                    buildDir = paramValue;
                }
            }
        }

        try
        {
            CurrentStagingEnvironment = (StagingEnvironment)Enum.Parse(typeof(StagingEnvironment), stagingEnvironment);
        }
        catch
        {
            Debug.LogWarning("Staging environment not defined in command line, defaulting to DEV: " + stagingEnvironment);
        }

        AssetDatabase.Refresh();
    }
}
