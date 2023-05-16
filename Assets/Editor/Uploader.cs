//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using UnityEngine.UI;
//using System.IO;
//using System;
//using UnityEngine.Networking;




//public class Uploader : BaseEditorWindow
//{
//    public string Number;

//    private string GetPrefabPath(string gameobjectTag)
//    {

//        var EnvironmentCanvas = GameObject.FindWithTag(gameobjectTag);

//        if (EnvironmentCanvas == null)
//        {
//            Debug.LogError($"CANT FIND ENVIRONEMNT CANVAS, PLEASE ENSURE THE ENVIRONMENT CANVAS USES TAG : {gameobjectTag}");
//            return null;
//        }
//        string EnvCanvasPath = ENVCANVASPATH + EnvironmentCanvas.name + ".prefab";
//        PrefabUtility.SaveAsPrefabAsset(EnvironmentCanvas, EnvCanvasPath);
//        AssetDatabase.Refresh();

//        return EnvCanvasPath;

//    }



//    private void OnGUI()
//    {

//        var entry = EditorGUILayout.TextField("Enter Page number :", Number);
//        Number = entry;

//        if (Number == "") return;

//        //if (GUILayout.Button("Clear bundles"))
//        //{
//        //    ClearBundles();
//        //}

//        if (GUILayout.Button("Upload Current Page"))
//        {
//            AssetBundleUtils.ClearBundles();

//            RecreateDirectory(EXPORTFOLDER);

//            var page = new Page();


//            page.pageNumber = int.Parse(Number);

//            var EnvironmentcanvasPath = GetPrefabPath(ENVIRONNMENTCAVAS);
//            page.PageBundleGuid = AssetBundleUtils.CreateNewBundle();

//            AssetBundleUtils.AddToBundle(EnvironmentcanvasPath, page.PageBundleGuid);


//            var SkyBoxPath = AssetDatabase.GetAssetPath(RenderSettings.skybox);
//            AssetBundleUtils.AddToBundle(SkyBoxPath, page.PageBundleGuid);

//            AssetBundleUtils.BuildBundles(EXPORTFOLDER);

//            //var pathJSON = $"{EXPORTFOLDER}/{pageNumberString}.json";
//            //var pageJSON = JsonUtility.ToJson(page, true);
//            //File.WriteAllText(pathJSON, pageJSON);

//            AssetDatabase.Refresh();

//            page.Texts = GetLanguageForPage(page.pageNumber);
//            var pathJSON = $"{EXPORTFOLDER}/Page_{page.pageNumber} JSON.json";
//            var pageJSON = JsonUtility.ToJson(page, true);
//            File.WriteAllText(pathJSON, pageJSON);



//            var files = Directory.GetFiles(EXPORTFOLDER);
//            DeleteManifestFiles(files);
//            string ASSETPATH = $"{Application.persistentDataPath}/../TaylorsTalesAssets/ChickenAndTheFox/Page_{page.pageNumber}";

//            RecreateDirectory(ASSETPATH);
//            AssetDatabase.Refresh();

//            var Exportfiles= Directory.GetFiles(EXPORTFOLDER);
//            foreach (string file in Exportfiles)
//            {

//                string source = Path.GetFullPath(file);
//                string target = $"{ASSETPATH}/{Path.GetFileName(file)}";

//                MoveFilesToDataPath(source, target);
          

//            }
//            AssetDatabase.Refresh();
//        }
//    }


//}








