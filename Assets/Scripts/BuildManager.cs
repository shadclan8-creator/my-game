using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

[InitializeOnLoad]
public class BuildManager : EditorWindow
{
    [MenuItem("Build/Build Windows %I")]
    public static void BuildWindows()
    {
        string appName = "TimesBaddestCat";
        string companyName = "BaddestCatGames";

        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPath = $"Builds/{appName}/",
            target = BuildTarget.StandaloneWindows64,
            subtarget = (int)StandaloneBuildSubtarget.Player,
            options = BuildOptions.CompressWithLzma | BuildOptions.DevelopmentBuild,
            scriptingBackend = ScriptingImplementation.Mono2x,
            developmentBuildType = BuildOptions.DevelopmentBuild,
        };

        // Create build directory
        System.IO.Directory.CreateDirectory("Builds");

        BuildSummary summary = BuildPipeline.BuildPlayer(buildOptions);
        BuildReport report = BuildReport.GetReport();

        Debug.Log($"Build complete!");
        Debug.Log($"Location: {summary.summary.totalSize} bytes");
        Debug.Log($"Warnings: {summary.summary.totalWarnings}");
        Debug.Log($"Errors: {summary.summary.totalErrors}");

        if (summary.summary.totalErrors == 0)
        {
            EditorUtility.RevealInFinder(buildOptions.locationPath);
            Debug.Log($"Build successful! Check: {buildOptions.locationPath}");
        }
        else
        {
            Debug.LogError($"Build failed with {summary.summary.totalErrors} errors!");
        }
    }

    private static string[] GetEnabledScenes()
    {
        return EditorBuildSettingsSceneList.GetActiveSceneList(EditorBuildSettings.scenes);
    }
}
