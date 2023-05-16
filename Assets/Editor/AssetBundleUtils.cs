using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetBundleUtils : MonoBehaviour
{
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
        var BundleGuid = Guid.NewGuid().ToString();
        return BundleGuid;
    }
    public static void AddToBundle(string path, string bundleGuid)
    {
        var importer = AssetImporter.GetAtPath(path);

        importer.SetAssetBundleNameAndVariant($"{bundleGuid}.unity3d", "");
    }


    public static void BuildBundles(string outputPath)
    {

        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(
          outputPath, BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.Android);

    }
}
