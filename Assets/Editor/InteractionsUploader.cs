using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using UnityEditor;
using System.IO;

public class InteractionsUploader : BaseEditorWindow
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

        if (GUILayout.Button($"Upload Interaction Canvas for page {Number}"))
        {
            AssetBundleUtils.ClearBundles();

            RecreateDirectory(EXPORTFOLDER);


            var EnvironmentcanvasPath = GetPrefabPath(INTERACTIONCANVAS, INTERACTIONSPATH);

            var bundleName = $"Page_{Number}_InteractionCanvas";

            AssetBundleUtils.AddToBundle(EnvironmentcanvasPath, bundleName);


            AssetBundleUtils.BuildBundles(EXPORTFOLDER);

            string DirectoryPath = $"{pagePath}/Page_{Number}";

            if (!Directory.Exists(DirectoryPath))
            {
                Directory.CreateDirectory(DirectoryPath);
            }



            string bundlePath = $"{DirectoryPath}/{bundleName}{bundleExtension}";
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
