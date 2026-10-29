using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Visual index of every page scene in the book: thumbnail, pipeline status, and shortcuts into the uploaders.
public class BookPagesWindow : BaseEditorWindow
{
    private const string SCENESPATH = "Assets/Scenes";
    private const string THUMBNAILPATH = "Library/TaylorsTales/Thumbnails";
    private const int ThumbWidth = 640, ThumbHeight = 360;
    private const float CardWidth = 232, CardThumbHeight = 130;

    private class PageEntry
    {
        public string path;
        public string name;
        public int number; // 0 = title page, -1 = not a page
        public bool hasEnvCanvas, hasInteractionCanvas;
        public FileInfo bundle;
        public bool hasText;
        public Texture2D thumbnail;

        public string Label => number == 0 ? "Title Page" : number > 0 ? $"Page {number}" : name;
        public bool Ready => hasEnvCanvas && hasInteractionCanvas;
    }

    private List<PageEntry> entries = new List<PageEntry>();
    private Vector2 scrollPos;
    private bool showOther;

    [MenuItem(MENUROOT + "Book Pages", priority = 0)]
    public static void ShowBookPagesWindow()
    {
        GetWindow<BookPagesWindow>("Book Pages");
    }

    [MenuItem(MENUROOT + "Recording Layout", priority = 61)]
    public static void RecordingLayout()
    {
        // dock every Taylor's Tales window as tabs in the Inspector's area so they sit beside the Scene view
        var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        var dockNextTo = inspectorType != null ? new[] { inspectorType } : new Type[0];

        GetWindow<FactUploader>("Facts", false, dockNextTo);
        GetWindow<EnvironmentUploader>("Environment", false, dockNextTo);
        GetWindow<TextPageUploader>("Page Text", false, dockNextTo);
        GetWindow<BookPagesWindow>("Book Pages", true, dockNextTo);
        Debug.Log("[Taylor's Tales] Recording layout ready. Save it with Window > Layouts > Save Layout... to reload it in one click.");
    }

    public override void OnEnable()
    {
        base.OnEnable();
        Refresh();
    }

    private void OnFocus()
    {
        if (ASSETPATH != null) RefreshStatus(); // pick up bundles/text made in the other windows
    }

    private void Refresh()
    {
        entries = AssetDatabase.FindAssets("t:Scene", new[] { SCENESPATH })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(p => new PageEntry { path = p, name = Path.GetFileNameWithoutExtension(p), number = PageNumberFromScene(Path.GetFileNameWithoutExtension(p)) })
            .OrderBy(e => e.number < 0 ? int.MaxValue : e.number).ThenBy(e => e.name)
            .ToList();

        foreach (var entry in entries)
        {
            // read tags straight from the scene yaml so we don't have to open every scene
            var yaml = File.ReadAllText(entry.path);
            entry.hasEnvCanvas = yaml.Contains($"m_TagString: {ENVIRONNMENTCANVAS}");
            entry.hasInteractionCanvas = yaml.Contains($"m_TagString: {INTERACTIONCANVAS}");
            entry.thumbnail = LoadThumbnail(entry.name);
        }
        RefreshStatus();
    }

    private void RefreshStatus()
    {
        var textPath = $"{PAGESPATH}/JSONPageData{jsonExtension}";
        var pagesWithText = new HashSet<int>();
        if (File.Exists(textPath))
        {
            var data = JsonUtility.FromJson<PageTextList>(File.ReadAllText(textPath));
            if (data?.pageTexts != null)
            {
                foreach (var page in data.pageTexts)
                    if (page.Texts != null && page.Texts.Any(t => !string.IsNullOrEmpty(t))) pagesWithText.Add(page.pageNumber);
            }
        }

        foreach (var entry in entries)
        {
            var bundlePath = $"{PAGESPATH}/page_{entry.number}_environmentcanvas{bundleExtension}";
            entry.bundle = File.Exists(bundlePath) ? new FileInfo(bundlePath) : null;
            entry.hasText = pagesWithText.Contains(entry.number);
        }
        Repaint();
    }

    // ---------- thumbnails ----------

    private static string ThumbnailFile(string sceneName) => $"{THUMBNAILPATH}/{sceneName}.png";

    private static Texture2D LoadThumbnail(string sceneName)
    {
        var file = ThumbnailFile(sceneName);
        if (!File.Exists(file)) return null;
        var tex = new Texture2D(2, 2) { hideFlags = HideFlags.HideAndDontSave };
        tex.LoadImage(File.ReadAllBytes(file));
        return tex;
    }

    private void CaptureThumbnails(IList<PageEntry> targets)
    {
        Directory.CreateDirectory(THUMBNAILPATH);
        var previousActive = SceneManager.GetActiveScene();
        try
        {
            for (int i = 0; i < targets.Count; i++)
            {
                var entry = targets[i];
                if (EditorUtility.DisplayCancelableProgressBar("Book Pages", $"Capturing {entry.Label}...", (float)i / targets.Count)) break;

                var scene = SceneManager.GetSceneByPath(entry.path);
                bool openedHere = !scene.isLoaded;
                if (openedHere) scene = EditorSceneManager.OpenScene(entry.path, OpenSceneMode.Additive);

                try
                {
                    SceneManager.SetActiveScene(scene); // the skybox comes from the active scene's lighting settings
                    CaptureScene(scene, ThumbnailFile(entry.name));
                }
                finally
                {
                    SceneManager.SetActiveScene(previousActive);
                    if (openedHere)
                    {
                        EditorSceneManager.CloseScene(scene, true);
                        EditorUtility.UnloadUnusedAssetsImmediate(); // each page is texture heavy, free it before loading the next
                    }
                }

                if (entry.thumbnail != null) DestroyImmediate(entry.thumbnail);
                entry.thumbnail = LoadThumbnail(entry.name);
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    private static void CaptureScene(Scene scene, string file)
    {
        var cameras = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Camera>(true)).ToArray();
        var cam = cameras.FirstOrDefault(c => c.CompareTag("MainCamera")) ?? cameras.FirstOrDefault();
        if (cam == null)
        {
            Debug.LogWarning($"[Book Pages] {scene.name} has no camera, skipping thumbnail.");
            return;
        }

        // overlay canvases don't render into a camera target, so point them at the camera for the capture
        var canvases = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Canvas>(true)).Where(c => c.isRootCanvas).ToArray();
        var saved = canvases.Select(c => (c.renderMode, c.worldCamera, c.planeDistance)).ToArray();

        var rt = RenderTexture.GetTemporary(ThumbWidth, ThumbHeight, 24, RenderTextureFormat.ARGB32);
        var previousTarget = cam.targetTexture;
        var previousScene = cam.scene;
        var previousActive = RenderTexture.active;
        try
        {
            cam.targetTexture = rt;
            cam.scene = scene; // only draw this page, not whatever else is open in the editor
            foreach (var canvas in canvases.Where(c => c.renderMode == RenderMode.ScreenSpaceOverlay))
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = cam;
                canvas.planeDistance = Mathf.Clamp(1f, cam.nearClipPlane + 0.01f, cam.farClipPlane);
            }
            foreach (var scaler in canvases.Select(c => c.GetComponent<CanvasScaler>()).Where(s => s != null && s.enabled))
            {
                scaler.enabled = false; scaler.enabled = true; // re-run scaling against the capture resolution
            }
            Canvas.ForceUpdateCanvases();

            cam.Render();

            RenderTexture.active = rt;
            var tex = new Texture2D(ThumbWidth, ThumbHeight, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, ThumbWidth, ThumbHeight), 0, 0);
            tex.Apply();
            File.WriteAllBytes(file, tex.EncodeToPNG());
            DestroyImmediate(tex);
        }
        finally
        {
            cam.targetTexture = previousTarget;
            cam.scene = previousScene;
            RenderTexture.active = previousActive;
            RenderTexture.ReleaseTemporary(rt);
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].renderMode = saved[i].renderMode;
                canvases[i].worldCamera = saved[i].worldCamera;
                canvases[i].planeDistance = saved[i].planeDistance;
            }
        }
    }

    // ---------- actions ----------

    private static bool OpenScene(PageEntry entry)
    {
        if (SceneManager.GetActiveScene().path == entry.path) return true;
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return false;
        EditorSceneManager.OpenScene(entry.path, OpenSceneMode.Single);
        return true;
    }

    private void BuildPage(PageEntry entry)
    {
        if (!OpenScene(entry)) return;
        var window = GetWindow<EnvironmentUploader>("Environment");
        window.Number = entry.number;
        window.Repaint();
    }

    // ---------- GUI ----------

    private void OnGUI()
    {
        var pages = entries.Where(e => e.number > 0).ToList();
        int built = pages.Count(e => e.bundle != null);
        int withText = pages.Count(e => e.hasText);

        DrawHeader("Book Pages", $"{BOOKNAME}  •  {pages.Count} pages  •  {built} built  •  {withText} with text");

        EditorGUILayout.BeginHorizontal();
        var progressRect = GUILayoutUtility.GetRect(100, 18, GUILayout.ExpandWidth(true));
        EditorGUI.ProgressBar(progressRect, pages.Count == 0 ? 0 : (float)built / pages.Count, $"{built} / {pages.Count} pages built");
        if (GUILayout.Button("Capture thumbnails", EditorStyles.miniButtonLeft, GUILayout.Width(120))) { CaptureThumbnails(entries); GUIUtility.ExitGUI(); }
        if (GUILayout.Button("Refresh", EditorStyles.miniButtonRight, GUILayout.Width(60))) Refresh();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(4);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        DrawGrid(entries.Where(e => e.number >= 0).ToList());

        var other = entries.Where(e => e.number < 0).ToList();
        if (other.Count > 0)
        {
            showOther = EditorGUILayout.Foldout(showOther, $"Other scenes ({other.Count})", true);
            if (showOther) DrawGrid(other);
        }
        EditorGUILayout.EndScrollView();
    }

    private void DrawGrid(List<PageEntry> list)
    {
        int perRow = Mathf.Max(1, (int)((position.width - 20) / (CardWidth + 8)));
        for (int i = 0; i < list.Count; i += perRow)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = i; j < Mathf.Min(i + perRow, list.Count); j++) DrawCard(list[j]);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawCard(PageEntry entry)
    {
        bool isOpen = SceneManager.GetActiveScene().path == entry.path;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(CardWidth));

        var thumbRect = GUILayoutUtility.GetRect(CardWidth - 8, CardThumbHeight, GUILayout.Width(CardWidth - 8), GUILayout.Height(CardThumbHeight));
        EditorGUI.DrawRect(thumbRect, new Color(0, 0, 0, 0.35f));
        if (entry.thumbnail != null)
        {
            GUI.DrawTexture(thumbRect, entry.thumbnail, ScaleMode.ScaleAndCrop);
        }
        else
        {
            GUI.Label(thumbRect, "No thumbnail yet", new GUIStyle(EditorStyles.centeredGreyMiniLabel) { alignment = TextAnchor.MiddleCenter });
        }
        if (isOpen)
        {
            var outline = AccentColour;
            EditorGUI.DrawRect(new Rect(thumbRect.x, thumbRect.y, thumbRect.width, 3), outline);
            EditorGUI.DrawRect(new Rect(thumbRect.x, thumbRect.yMax - 3, thumbRect.width, 3), outline);
            EditorGUI.DrawRect(new Rect(thumbRect.x, thumbRect.y, 3, thumbRect.height), outline);
            EditorGUI.DrawRect(new Rect(thumbRect.xMax - 3, thumbRect.y, 3, thumbRect.height), outline);
        }
        if (Event.current.type == EventType.MouseDown && Event.current.clickCount == 2 && thumbRect.Contains(Event.current.mousePosition))
        {
            OpenScene(entry);
            GUIUtility.ExitGUI();
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(entry.Label, new GUIStyle(EditorStyles.boldLabel) { fontSize = 13 });
        GUILayout.FlexibleSpace();
        if (isOpen) GUILayout.Label("OPEN", new GUIStyle(EditorStyles.miniBoldLabel) { normal = { textColor = AccentColour } });
        EditorGUILayout.EndHorizontal();

        if (entry.number > 0)
        {
            StatusRow(entry.Ready, entry.Ready ? "Canvases tagged" : "Missing tagged canvas");
            StatusRow(entry.hasText, entry.hasText ? "Text written" : "No text yet", optional: true);
            StatusRow(entry.bundle != null, entry.bundle != null ? $"Built  {FormatBytes(entry.bundle.Length)}  {entry.bundle.LastWriteTime:dd MMM HH:mm}" : "Not built", optional: true);
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Open", EditorStyles.miniButtonLeft)) { OpenScene(entry); GUIUtility.ExitGUI(); }
        EditorGUI.BeginDisabledGroup(entry.number <= 0);
        if (GUILayout.Button("Text", EditorStyles.miniButtonMid)) TextPageUploader.OpenAtPage(entry.number);
        EditorGUI.BeginDisabledGroup(!entry.Ready);
        if (GUILayout.Button("Build", EditorStyles.miniButtonRight)) { BuildPage(entry); GUIUtility.ExitGUI(); }
        EditorGUI.EndDisabledGroup();
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        GUILayout.Space(4);
    }

    private void OnDisable()
    {
        foreach (var entry in entries)
            if (entry.thumbnail != null) DestroyImmediate(entry.thumbnail);
    }
}
