using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

public enum Languages
{
    English = 0,
    Irish = 1,
    French = 2,
    Spanish = 3
}
public class BaseEditorWindow : EditorWindow
{

    [Header("Paths")]

    protected string ASSETPATH; // uses persistent data path and cant be used in declaration, declared in onenable
    protected const string FACTPATH = "Assets/StreamingAssets/Facts";
    protected const string LANGUAGESPATH = "Assets/StreamingAssets/PageTexts";
    protected const string EXPORTFOLDER = "Assets/../ExportFolder";
    public const string ENVCANVASPATH = "Assets/Prefabs/EnvironmentCanvas/";
    public const string INTERACTIONSPATH = "Assets/Prefabs/InteractionCanvas/";
    public const string FACTIMAGES = "Assets/FactImages";

    [Header("Tags")]
    public const string ENVIRONNMENTCANVAS = "StaticEnvironmentCanvas";
    public const string INTERACTIONCANVAS = "InteractionCanvas";

    [Header("Extensions")]
    protected const string jsonExtension = ".json";
    protected const string manifestExtension = ".manifest";
    protected const string metaExtension = ".meta";
    protected const string bundleExtension = ".unity3d";

    [Header("languageUtils")]
    protected Languages CurrentLanguageIndex = Languages.English;
    protected const int LANGUAGESAVAILABLE = 4;


    protected string getLanguageName(Languages language)
    {
        switch (language)
        {
            case Languages.English: return "English";
            case Languages.Irish: return "Irish";
            case Languages.French: return "French";
            case Languages.Spanish: return "Spanish";
        }
        Debug.Log("Language not in list");
        return null;
    }
  

    protected void RecreateDirectory(string directory)
    {

        if (Directory.Exists(directory)) { Directory.Delete(directory, true); }
        Directory.CreateDirectory(directory);
    }
    protected LocalisedData GetLanguages(Languages index)
    {
        string langName = getLanguageName(index);
        Debug.Log(langName);
        string fileName = $"{LANGUAGESPATH}/{langName}.txt";

        if (File.Exists(fileName))
        {
            var text = File.ReadAllText(fileName);

            var loadeddata = JsonUtility.FromJson<LocalisedData>(text);
            return loadeddata;
        }
        else
        {
            return null;
        }
    }

    public virtual void OnEnable()
    {
        ASSETPATH = $"{Application.persistentDataPath}/../TaylorsTalesAssets/ChickenAndTheFox";
        string FactPath = $"{ASSETPATH}/Facts";
        if (!Directory.Exists(ASSETPATH))
        {
            Directory.CreateDirectory(ASSETPATH);
        }

    }

    protected string[] getFileswithoutMetaManifestFiles(string directoryPath)
    {
        return Directory.GetFiles(directoryPath).Where(T => // get files in folder that arent manifest or meta files
 !Path.GetExtension(T).Equals(".unity3d.manifest") ||
 !Path.GetExtension(T).Equals(".manifest") ||
 !Path.GetExtension(T).Equals(".meta")).ToArray();
    }

    protected string[] getJPGsorPNGs(string directoryPath) // should only accept png really
    {
        return Directory.GetFiles(directoryPath).Where(T => // get files in folder that arent manifest or meta files
 Path.GetExtension(T).Equals(".jpg") || Path.GetExtension(T).Equals(".JPG") || Path.GetExtension(T).Equals(".PNG") ||
 Path.GetExtension(T).Equals(".png")).ToArray();
    }

    protected void CreateDirectoryIfDoesntExist(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }
    }


    protected void MoveFiles(string source, string target)
    {
        File.Move(source, target);
    }

    protected List<String> GetLanguageForPage(int pageNumber)
    {
        var languageListJSON = new List<string>();
        var languages = Directory.GetFiles(LANGUAGESPATH);

        foreach (string file in languages)
        {
            if (Path.GetExtension(file).Equals(".txt"))
            {
                var text = File.ReadAllText(file);
                var loadeddata = JsonUtility.FromJson<LocalisedData>(text);
                languageListJSON.Add(loadeddata.items[pageNumber - 1].value);
            }

        }

        return languageListJSON;

    }



    protected void DeleteFile(string filepath)
    {
        File.Delete(filepath);

    }

    protected void DisplayLanguageButtons()
    {

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("English"))
        {
            CurrentLanguageIndex = Languages.English;
            Repaint();
        }

        if (GUILayout.Button("Irish"))
        {
            CurrentLanguageIndex = Languages.Irish;
            Repaint();
        }
        if (GUILayout.Button("French"))
        {
            CurrentLanguageIndex = Languages.French;
            Repaint();
        }
        if (GUILayout.Button("Spanish"))
        {
            CurrentLanguageIndex = Languages.Spanish;
            Repaint();
        }
        EditorGUILayout.EndHorizontal();
    }

    protected string GetPrefabPath(string gameobjectTag, string prefabPath)
    {

        var EnvironmentCanvas = GameObject.FindWithTag(gameobjectTag);

        if (EnvironmentCanvas == null)
        {
            Debug.LogError($"CANT FIND ENVIRONEMNT CANVAS, PLEASE ENSURE THE ENVIRONMENT CANVAS USES TAG : {gameobjectTag}");
            return null;
        }
        string CanvasPath = prefabPath + EnvironmentCanvas.name + ".prefab";
        if (File.Exists(CanvasPath))
        {
            File.Delete(CanvasPath);
            AssetDatabase.Refresh();
        }
        PrefabUtility.SaveAsPrefabAsset(EnvironmentCanvas, CanvasPath);

        AssetDatabase.Refresh();
        return CanvasPath;

    }



    [MenuItem("EditorWindows/Fact uploader")]
    public static void ShowFactUploadWindow()
    {
        GetWindow<FactUploader>("Fact Uploader");
    }

    [MenuItem("EditorWindows/Environment uploader")]
    public static void ShowPageUploadWindow()
    {
        GetWindow<EnvironmentUploader>("Environment uploader");
    }

    [MenuItem("EditorWindows/PageText uploader")]
    public static void ShowPageTextUploadWindow()
    {
        GetWindow<TextPageUploader>("PageText uploader");
    }


    //[MenuItem("EditorWindows/InteractionCanvas uploader")]
    //public static void ShowPInteractionsUploadWindow()
    //{
    //    GetWindow<InteractionsUploader>("Interaction uploader");
    //}
}
