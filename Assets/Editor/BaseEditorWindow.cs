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
    protected const string MENUROOT = "Taylor's Tales/";
    protected const string BOOKNAME = "ChickenAndTheFox";

    [Header("Paths")]

    protected string ASSETPATH; // uses persistent data path and cant be used in declaration, declared in onenable
    protected string PAGESPATH => $"{ASSETPATH}/Pages";
    protected string FACTSOUTPUTPATH => $"{ASSETPATH}/Facts";
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

    protected static readonly Color AccentColour = new Color(0.24f, 0.49f, 0.91f); // matches the editor's selection blue
    protected static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg" };

    protected string getLanguageName(Languages language)
    {
        return Enum.IsDefined(typeof(Languages), language) ? language.ToString() : null;
    }

    protected void RecreateDirectory(string directory)
    {
        if (Directory.Exists(directory)) { Directory.Delete(directory, true); }
        Directory.CreateDirectory(directory);
    }

    protected LocalisedData GetLanguages(Languages index)
    {
        string fileName = $"{LANGUAGESPATH}/{getLanguageName(index)}.txt";
        if (!File.Exists(fileName)) return null;

        return JsonUtility.FromJson<LocalisedData>(File.ReadAllText(fileName));
    }

    public virtual void OnEnable()
    {
        ASSETPATH = Path.GetFullPath($"{Application.persistentDataPath}/../TaylorsTalesAssets/{BOOKNAME}");
        Directory.CreateDirectory(PAGESPATH);
        Directory.CreateDirectory(FACTSOUTPUTPATH);
    }

    protected string[] getFileswithoutMetaManifestFiles(string directoryPath)
    {
        return Directory.GetFiles(directoryPath).Where(T => // get files in folder that arent manifest or meta files
            !T.EndsWith(manifestExtension, StringComparison.OrdinalIgnoreCase) &&
            !T.EndsWith(metaExtension, StringComparison.OrdinalIgnoreCase)).ToArray();
    }

    protected string[] getJPGsorPNGs(string directoryPath)
    {
        if (!Directory.Exists(directoryPath)) return new string[0];

        return Directory.GetFiles(directoryPath)
            .Where(T => ImageExtensions.Contains(Path.GetExtension(T).ToLowerInvariant()))
            .Select(T => T.Replace('\\', '/'))
            .ToArray();
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
        if (File.Exists(target)) File.Delete(target);
        File.Move(source, target);
    }

    protected void DeleteFile(string filepath)
    {
        if (File.Exists(filepath)) File.Delete(filepath);
    }

    protected void DisplayLanguageButtons(Action callback = null, Func<Languages, bool> hasContent = null)
    {
        var labels = Enum.GetValues(typeof(Languages)).Cast<Languages>()
            .Select(l => hasContent == null ? l.ToString() : $"{l} {(hasContent(l) ? "●" : "○")}")
            .ToArray();

        var selected = (Languages)GUILayout.Toolbar((int)CurrentLanguageIndex, labels, GUILayout.Height(24));
        if (selected != CurrentLanguageIndex)
        {
            CurrentLanguageIndex = selected;
            GUI.FocusControl(null);
            callback?.Invoke();
            Repaint();
        }
    }

    protected GameObject FindTagged(string gameobjectTag)
    {
        try { return GameObject.FindWithTag(gameobjectTag); }
        catch (UnityException) { return null; } // tag not defined in TagManager
    }

    protected string GetPrefabPath(string gameobjectTag, string prefabPath)
    {
        var canvas = FindTagged(gameobjectTag);

        if (canvas == null)
        {
            Debug.LogError($"Can't find a canvas tagged '{gameobjectTag}' in the open scene.");
            return null;
        }

        Directory.CreateDirectory(prefabPath);
        string CanvasPath = prefabPath + canvas.name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(canvas, CanvasPath); // overwriting in place keeps the prefab guid stable
        AssetDatabase.Refresh();
        return CanvasPath;
    }

    // ---------- shared UI ----------

    protected void DrawHeader(string title, string subtitle)
    {
        EditorGUILayout.Space(6);
        GUILayout.Label(title, new GUIStyle(EditorStyles.boldLabel) { fontSize = 15 });
        GUILayout.Label(subtitle, EditorStyles.miniLabel);

        var line = EditorGUILayout.GetControlRect(false, 1);
        EditorGUI.DrawRect(line, new Color(0.5f, 0.5f, 0.5f, 0.35f));
        EditorGUILayout.Space(4);
    }

    protected void DrawOutputInfo(string folder)
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        var target = AssetBundleUtils.SelectedTarget;
        var installed = AssetBundleUtils.IsTargetInstalled(target);
        GUILayout.Label(EditorGUIUtility.IconContent(installed ? "TestPassed" : "console.warnicon.sml"), GUILayout.Width(18), GUILayout.Height(18));
        GUILayout.Label(installed ? $"Bundles: {target}" : $"Bundles: {target} not installed, using {EditorUserBuildSettings.activeBuildTarget}", EditorStyles.miniLabel);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Open output folder", EditorStyles.miniButton))
        {
            Directory.CreateDirectory(folder);
            EditorUtility.RevealInFinder(folder + "/");
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField(folder, EditorStyles.centeredGreyMiniLabel);
    }

    protected void StatusRow(bool ok, string label, bool optional = false)
    {
        EditorGUILayout.BeginHorizontal();
        var icon = ok ? "TestPassed" : optional ? "console.warnicon.sml" : "TestFailed";
        GUILayout.Label(EditorGUIUtility.IconContent(icon), GUILayout.Width(18), GUILayout.Height(18));
        GUILayout.Label(label);
        EditorGUILayout.EndHorizontal();
    }

    internal static bool SuppressDialogs; // set by automated runs so modal dialogs don't block the editor

    protected void ShowError(string title, string message)
    {
        Debug.LogError($"[{title}] {message}");
        if (!SuppressDialogs) EditorUtility.DisplayDialog(title, message, "OK");
    }

    protected void ShowResult(string title, string message, string folder)
    {
        Debug.Log($"[{title}] {message.Replace("\n\n", " ")}");
        if (SuppressDialogs) return;
        if (EditorUtility.DisplayDialog(title, message, "Open folder", "OK"))
        {
            EditorUtility.RevealInFinder(folder + "/");
        }
    }

    // "page 11" -> 11, "Page4" -> 4, "TitlePage" -> 0, anything else -> -1
    protected static int PageNumberFromScene(string sceneName)
    {
        if (sceneName.Equals("TitlePage", StringComparison.OrdinalIgnoreCase)) return 0;
        var match = System.Text.RegularExpressions.Regex.Match(sceneName, @"^page\s*(\d+)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        return match.Success ? int.Parse(match.Groups[1].Value) : -1;
    }

    protected static string FormatBytes(long bytes)
    {
        if (bytes > 1024 * 1024) return $"{bytes / (1024f * 1024f):0.0} MB";
        return $"{bytes / 1024f:0} KB";
    }

    // ---------- menu ----------

    [MenuItem(MENUROOT + "1. Page Text", priority = 1)]
    public static void ShowPageTextUploadWindow()
    {
        GetWindow<TextPageUploader>("Page Text");
    }

    [MenuItem(MENUROOT + "2. Environment Uploader", priority = 2)]
    public static void ShowPageUploadWindow()
    {
        GetWindow<EnvironmentUploader>("Environment");
    }

    [MenuItem(MENUROOT + "3. Fact Uploader", priority = 3)]
    public static void ShowFactUploadWindow()
    {
        GetWindow<FactUploader>("Facts");
    }

    [MenuItem(MENUROOT + "Tools/Word List", priority = 20)]
    public static void ShowWordListGeneratorWindow()
    {
        GetWindow<WordListGenerator>("Word List");
    }

    [MenuItem(MENUROOT + "Tools/Sentence List", priority = 21)]
    public static void ShowSentenceListGeneratorWindow()
    {
        GetWindow<SentenceListGenerator>("Sentence List");
    }

    [MenuItem(MENUROOT + "Tools/Bundle Target/Android (release)", priority = 40)]
    static void TargetAndroid() => AssetBundleUtils.SelectedTarget = BuildTarget.Android;

    [MenuItem(MENUROOT + "Tools/Bundle Target/Android (release)", true)]
    static bool TargetAndroidCheck() { Menu.SetChecked(MENUROOT + "Tools/Bundle Target/Android (release)", AssetBundleUtils.SelectedTarget == BuildTarget.Android); return true; }

    [MenuItem(MENUROOT + "Tools/Bundle Target/Windows (local testing)", priority = 41)]
    static void TargetWindows() => AssetBundleUtils.SelectedTarget = BuildTarget.StandaloneWindows64;

    [MenuItem(MENUROOT + "Tools/Bundle Target/Windows (local testing)", true)]
    static bool TargetWindowsCheck() { Menu.SetChecked(MENUROOT + "Tools/Bundle Target/Windows (local testing)", AssetBundleUtils.SelectedTarget == BuildTarget.StandaloneWindows64); return true; }

    [MenuItem(MENUROOT + "Open Output Folder", priority = 60)]
    static void OpenOutput()
    {
        var path = Path.GetFullPath($"{Application.persistentDataPath}/../TaylorsTalesAssets/{BOOKNAME}");
        Directory.CreateDirectory(path);
        EditorUtility.RevealInFinder(path + "/");
    }
}
