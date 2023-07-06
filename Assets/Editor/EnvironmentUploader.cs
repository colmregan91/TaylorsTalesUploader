using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Linq;

public class EnvironmentUploader : BaseEditorWindow
{
    public string Number;
    private string pagePath;

    public override void OnEnable()
    {
        base.OnEnable();
        pagePath = $"{ASSETPATH}/Pages";
    }
    private void OnGUI()
    {

        var entry = EditorGUILayout.TextField("Enter Page number :", Number);
        Number = entry;

        if (Number == "") return;

        //if (GUILayout.Button("Clear bundles"))
        //{
        //    ClearBundles();
        //}

        if (GUILayout.Button($"Upload Environment for page {Number}"))
        {
            AssetBundleUtils.ClearBundles();

            RecreateDirectory(EXPORTFOLDER);


            var EnvironmentcanvasPath = GetPrefabPath(ENVIRONNMENTCANVAS, ENVCANVASPATH);
            var InteractioncanvasPath = GetPrefabPath(INTERACTIONCANVAS, INTERACTIONSPATH);

            var bundleName = $"Page_{Number}_EnvironmentCanvas";

            AssetBundleUtils.AddToBundle(EnvironmentcanvasPath, bundleName);
            AssetBundleUtils.AddToBundle(InteractioncanvasPath, bundleName);

            var SkyBoxPath = AssetDatabase.GetAssetPath(RenderSettings.skybox);
            AssetBundleUtils.AddToBundle(SkyBoxPath, bundleName);

            AssetBundleUtils.BuildBundles(EXPORTFOLDER);




            string bundlePath = $"{pagePath}/{bundleName}{bundleExtension}";
            if (File.Exists(bundlePath))
            {
                File.Delete(bundlePath);
            }

            AssetDatabase.Refresh();


            string source = Path.GetFullPath($"{EXPORTFOLDER}/{bundleName}{bundleExtension}");

            MoveFiles(source, bundlePath);

            AssetDatabase.Refresh();
        }
    }
}
