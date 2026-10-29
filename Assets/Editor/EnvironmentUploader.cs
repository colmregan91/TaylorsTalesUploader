using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnvironmentUploader : BaseEditorWindow
{
    public int Number = 1;
    private Vector2 scrollPos;

    private string BundleName => $"page_{Number}_environmentcanvas";

    private string lastScenePath;

    private void OnFocus() => Repaint(); // re-check the scene when the user comes back from editing it
    private void OnHierarchyChange() => Repaint();

    private void OnGUI()
    {
        // follow the open scene, "page 11" fills in page 11
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (scene.path != lastScenePath)
        {
            lastScenePath = scene.path;
            var parsed = PageNumberFromScene(scene.name);
            if (parsed > 0) Number = parsed;
        }

        DrawHeader("Environment Uploader", "Packages the open scene's canvases and skybox into a page bundle");
        DrawOutputInfo(PAGESPATH);

        EditorGUILayout.Space();
        Number = Mathf.Max(1, EditorGUILayout.IntField("Page number", Number));
        EditorGUILayout.LabelField("Bundle name", BundleName + bundleExtension);

        var envCanvas = FindTagged(ENVIRONNMENTCANVAS);
        var interactionCanvas = FindTagged(INTERACTIONCANVAS);
        var skybox = RenderSettings.skybox;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Scene check", EditorStyles.boldLabel);
        StatusRow(envCanvas != null, envCanvas != null ? $"Environment canvas: {envCanvas.name}" : $"No object tagged '{ENVIRONNMENTCANVAS}'");
        StatusRow(interactionCanvas != null, interactionCanvas != null ? $"Interaction canvas: {interactionCanvas.name}" : $"No object tagged '{INTERACTIONCANVAS}'");
        StatusRow(skybox != null, skybox != null ? $"Skybox: {skybox.name}" : "No skybox set in Lighting settings (optional)", optional: true);

        EditorGUILayout.Space();
        bool ready = envCanvas != null && interactionCanvas != null;
        EditorGUI.BeginDisabledGroup(!ready);
        GUI.backgroundColor = ready ? new Color(0.55f, 0.9f, 0.55f) : Color.white;
        if (GUILayout.Button($"Build environment for page {Number}", GUILayout.Height(34)))
        {
            Upload(skybox);
            GUIUtility.ExitGUI();
        }
        GUI.backgroundColor = Color.white;
        EditorGUI.EndDisabledGroup();

        DrawExistingBundles();
    }

    private void Upload(Material skybox)
    {
        string bundlePath = $"{PAGESPATH}/{BundleName}{bundleExtension}";
        try
        {
            EditorUtility.DisplayProgressBar("Environment Uploader", "Saving canvases as prefabs...", 0.1f);
            AssetBundleUtils.ClearBundles();
            RecreateDirectory(EXPORTFOLDER);

            var EnvironmentcanvasPath = GetPrefabPath(ENVIRONNMENTCANVAS, ENVCANVASPATH);
            var InteractioncanvasPath = GetPrefabPath(INTERACTIONCANVAS, INTERACTIONSPATH);

            bool ok = AssetBundleUtils.AddToBundle(EnvironmentcanvasPath, BundleName)
                    & AssetBundleUtils.AddToBundle(InteractioncanvasPath, BundleName);
            var SkyBoxPath = skybox != null ? AssetDatabase.GetAssetPath(skybox) : null;
            if (SkyBoxPath != null && SkyBoxPath.StartsWith("Assets/")) AssetBundleUtils.AddToBundle(SkyBoxPath, BundleName); // built-in skyboxes can't be bundled
            if (!ok) return;

            EditorUtility.DisplayProgressBar("Environment Uploader", $"Building {BundleName}...", 0.4f);
            var manifest = AssetBundleUtils.BuildBundles(EXPORTFOLDER);

            string source = Path.GetFullPath($"{EXPORTFOLDER}/{BundleName}{bundleExtension}");
            if (manifest == null || !File.Exists(source))
            {
                ShowError("Environment Uploader", "Bundle build failed, check the console for details.");
                return;
            }

            EditorUtility.DisplayProgressBar("Environment Uploader", "Moving bundle to output folder...", 0.9f);
            MoveFiles(source, bundlePath);
        }
        finally
        {
            AssetBundleUtils.ClearBundles();
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        ShowResult("Environment Uploader", $"Page {Number} built.\n\n{Path.GetFileName(bundlePath)}  ({FormatBytes(new FileInfo(bundlePath).Length)})", PAGESPATH);
    }

    private void DrawExistingBundles()
    {
        if (!Directory.Exists(PAGESPATH)) return;
        var bundles = Directory.GetFiles(PAGESPATH, "*" + bundleExtension)
            .Select(f => new FileInfo(f))
            .OrderBy(f => f.Name.Length).ThenBy(f => f.Name)
            .ToArray();
        if (bundles.Length == 0) return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Built pages ({bundles.Length})", EditorStyles.boldLabel);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var bundle in bundles)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label(bundle.Name);
            GUILayout.FlexibleSpace();
            GUILayout.Label($"{FormatBytes(bundle.Length)}   {bundle.LastWriteTime:dd MMM HH:mm}", EditorStyles.miniLabel);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }
}
