using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AssetBundleUtils
{
    // The reader app ships on Android, so that is the preferred bundle target.
    // If the Android module isn't installed in this editor we fall back to the active target
    // so the pipeline can still be exercised end to end.
    public const BuildTarget PreferredTarget = BuildTarget.Android;
    private const string TargetPrefKey = "TaylorsTales.BundleTarget";

    public static BuildTarget SelectedTarget
    {
        get => (BuildTarget)EditorPrefs.GetInt(TargetPrefKey, (int)PreferredTarget);
        set => EditorPrefs.SetInt(TargetPrefKey, (int)value);
    }

    public static bool IsTargetInstalled(BuildTarget target)
    {
        return BuildPipeline.IsBuildTargetSupported(BuildPipeline.GetBuildTargetGroup(target), target);
    }

    public static BuildTarget ResolveBuildTarget()
    {
        var target = SelectedTarget;
        if (IsTargetInstalled(target)) return target;

        var fallback = EditorUserBuildSettings.activeBuildTarget;
        Debug.LogWarning($"[Taylor's Tales] {target} build support is not installed, building bundles for {fallback} instead.");
        return fallback;
    }

    public static void ClearBundles()
    {
        var existingBundles = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < existingBundles.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(existingBundles[i], true);
        }
        AssetDatabase.Refresh();
    }

    public static string CreateNewBundle()
    {
        return Guid.NewGuid().ToString();
    }

    public static bool AddToBundle(string path, string bundleName)
    {
        var importer = string.IsNullOrEmpty(path) ? null : AssetImporter.GetAtPath(path);
        if (importer == null)
        {
            Debug.LogError($"[Taylor's Tales] Can't add '{path}' to bundle {bundleName}, asset not found.");
            return false;
        }

        importer.SetAssetBundleNameAndVariant($"{bundleName}.unity3d", "");
        return true;
    }

    public static AssetBundleManifest BuildBundles(string outputPath)
    {
        Directory.CreateDirectory(outputPath);
        return BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ForceRebuildAssetBundle, ResolveBuildTarget());
    }
}
