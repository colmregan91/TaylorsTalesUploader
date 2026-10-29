using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

public class FactUploader : BaseEditorWindow
{
    bool[] showList;
    bool[] infoList;
    bool[] imageList;
    List<Fact> factList = new List<Fact>();
    Dictionary<string, Texture2D[]> thumbnails = new Dictionary<string, Texture2D[]>();
    Vector2 scrollPos;

    private const float ThumbSize = 96;

    public override void OnEnable()
    {
        base.OnEnable();
        DisplayFacts();
    }

    private string ImageFolder(Fact fact) => $"{FACTIMAGES}/{fact.TriggerWords[(int)Languages.English]}";

    private Texture2D[] GetThumbnails(Fact fact)
    {
        var folder = ImageFolder(fact);
        if (!thumbnails.TryGetValue(folder, out var textures))
        {
            textures = getJPGsorPNGs(folder).Select(AssetDatabase.LoadAssetAtPath<Texture2D>).Where(t => t != null).ToArray();
            thumbnails[folder] = textures;
        }
        return textures;
    }

    void ShowFact(Fact fact, int factNumber)
    {
        var images = GetThumbnails(fact);
        int languagesWithInfo = fact.FactInfo.Count(s => !string.IsNullOrEmpty(s));

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal();
        showList[factNumber] = EditorGUILayout.Foldout(showList[factNumber], $"Fact {factNumber + 1} - {fact.TriggerWords[(int)Languages.English]}", true, EditorStyles.foldoutHeader);
        GUILayout.FlexibleSpace();
        GUILayout.Label(EditorGUIUtility.IconContent(images.Length > 0 ? "TestPassed" : "console.warnicon.sml"), GUILayout.Width(18), GUILayout.Height(18));
        GUILayout.Label($"{images.Length} image(s)  •  {languagesWithInfo}/{LANGUAGESAVAILABLE} languages", EditorStyles.miniLabel, GUILayout.Width(170));
        EditorGUILayout.EndHorizontal();

        if (showList[factNumber])
        {
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(true);
            foreach (Languages lang in Enum.GetValues(typeof(Languages)))
            {
                EditorGUILayout.TextField($"Trigger Word {lang}", fact.TriggerWords[(int)lang]);
            }
            EditorGUI.EndDisabledGroup();

            infoList[factNumber] = EditorGUILayout.Foldout(infoList[factNumber], "Fact Information", true);

            if (infoList[factNumber])
            {
                var areaStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = true };
                foreach (Languages lang in Enum.GetValues(typeof(Languages)))
                {
                    GUILayout.Label(lang.ToString());
                    fact.FactInfo[(int)lang] = EditorGUILayout.TextArea(fact.FactInfo[(int)lang], areaStyle, GUILayout.MinHeight(40));
                }
            }

            imageList[factNumber] = EditorGUILayout.Foldout(imageList[factNumber], $"Fact Images ({images.Length})", true);

            if (imageList[factNumber])
            {
                string folder = ImageFolder(fact);
                if (images.Length == 0)
                {
                    EditorGUILayout.HelpBox(Directory.Exists(folder) ? $"No images found in {folder}" : $"No image folder at {folder}", MessageType.Warning);
                }
                else
                {
                    DrawThumbnailGrid(images);
                }
            }
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();
    }

    private void DrawThumbnailGrid(Texture2D[] images)
    {
        int perRow = Mathf.Max(1, (int)((position.width - 60) / (ThumbSize + 6)));
        for (int i = 0; i < images.Length; i += perRow)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            for (int j = i; j < Mathf.Min(i + perRow, images.Length); j++)
            {
                var rect = GUILayoutUtility.GetRect(ThumbSize, ThumbSize, GUILayout.Width(ThumbSize), GUILayout.Height(ThumbSize));
                EditorGUI.DrawRect(rect, new Color(0, 0, 0, 0.2f));
                GUI.DrawTexture(rect, images[j], ScaleMode.ScaleToFit);
                if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                {
                    EditorGUIUtility.PingObject(images[j]);
                    Selection.activeObject = images[j];
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private string[] getSpecialWordsForLanguage(Languages languageIndex)
    {
        var languagedata = GetLanguages(languageIndex);

        if (languagedata == null)
        {
            return null;
        }
        List<string> specialList = new List<string>();

        for (int i = 0; i < languagedata.items.Length; i++)
        {
            var specials = languagedata.items[i].value.Split(' ', '\n', '.', '“', '”', ',', '?');
            for (int j = 0; j < specials.Length; j++)
            {
                if (specials[j].Contains("<color=red>"))
                {
                    var TriggerWord = specials[j].Replace("<color=red>", "").Replace("</color=red>", "");
                    specialList.Add(TriggerWord);
                }
            }
        }

        return specialList.ToArray();
    }

    private void DisplayFacts()
    {
        factList.Clear();
        thumbnails.Clear();

        var specialListEnglish = getSpecialWordsForLanguage(Languages.English);
        var SpecialListIrish = getSpecialWordsForLanguage(Languages.Irish);

        if (specialListEnglish != null)
        {
            var factDataEnglish = GetFacts(Languages.English);

            for (int i = 0; i < specialListEnglish.Length; i++)
            {
                Fact fact = new Fact();
                fact.TriggerWords = new string[LANGUAGESAVAILABLE];
                fact.FactInfo = new string[LANGUAGESAVAILABLE];
                createFactForLanguage(specialListEnglish[i], factDataEnglish, (int)Languages.English, fact);
                factList.Add(fact);
            }
        }
        else
        {
            Debug.LogWarning($"{Languages.English} not currently supported");
        }

        if (SpecialListIrish != null)
        {
            var factDataIrish = GetFacts(Languages.Irish);
            // trigger words appear in the same order in each translation, so index i lines up with the English fact
            for (int i = 0; i < Mathf.Min(SpecialListIrish.Length, factList.Count); i++)
            {
                createFactForLanguage(SpecialListIrish[i], factDataIrish, (int)Languages.Irish, factList[i]);
            }
        }
        else
        {
            Debug.LogWarning($"{Languages.Irish} not currently supported");
        }

        showList = new bool[factList.Count];
        imageList = new bool[factList.Count];
        infoList = new bool[factList.Count];
    }

    private void createFactForLanguage(string specWord, LocalisedData factlist, int langIndex, Fact newFact)
    {
        string word = specWord.ToLower();
        newFact.TriggerWords[langIndex] = word;
        if (factlist == null) return;

        string english = newFact.TriggerWords[(int)Languages.English];
        LocalisedItem factInfo = factlist.items.FirstOrDefault(T => T.key.Equals(english));

        if (factInfo == null)
        {
            Debug.LogWarning($"No {(Languages)langIndex} fact configured for '{english}' (add an entry with key \"{english}\")");
            return;
        }
        newFact.FactInfo[langIndex] = factInfo.value;
    }

    private LocalisedData GetFacts(Languages index)
    {
        string fileName = $"{FACTPATH}/Facts{getLanguageName(index)}.txt";
        if (!File.Exists(fileName)) return null;

        return JsonUtility.FromJson<LocalisedData>(File.ReadAllText(fileName));
    }

    private void Upload()
    {
        string FactPath = FACTSOUTPUTPATH;
        FactList factlist = new FactList(factList);
        int bundled = 0;

        try
        {
            AssetBundleUtils.ClearBundles();
            RecreateDirectory(FactPath);

            for (int i = 0; i < factlist.Facts.Count; i++)
            {
                var fact = factlist.Facts[i];
                EditorUtility.DisplayProgressBar("Fact Uploader", $"Tagging images for '{fact.TriggerWords[(int)Languages.English]}'", (float)i / factlist.Facts.Count * 0.3f);

                var files = getJPGsorPNGs(ImageFolder(fact));
                if (files.Length == 0)
                {
                    fact.imagesBundle = string.Empty;
                    continue;
                }

                var bundleGuid = AssetBundleUtils.CreateNewBundle();
                fact.imagesBundle = bundleGuid;
                foreach (var file in files)
                {
                    AssetBundleUtils.AddToBundle(file, bundleGuid);
                }
                bundled++;
            }

            EditorUtility.DisplayProgressBar("Fact Uploader", "Building image bundles...", 0.4f);
            if (bundled > 0 && AssetBundleUtils.BuildBundles(FactPath) == null)
            {
                ShowError("Fact Uploader", "Bundle build failed, check the console for details.");
                return;
            }

            File.WriteAllText($"{FactPath}/Facts.json", JsonUtility.ToJson(factlist, true));
        }
        finally
        {
            AssetBundleUtils.ClearBundles();
            EditorUtility.ClearProgressBar();
        }

        ShowResult("Fact Uploader", $"Uploaded {factlist.Facts.Count} facts with {bundled} image bundle(s).\n\nFacts.json written to the Facts folder.", FactPath);
    }

    private void OnGUI()
    {
        DrawHeader("Fact Uploader", $"Bundles fact images and writes Facts.json for {BOOKNAME}");
        DrawOutputInfo(FACTSOUTPUTPATH);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label($"{factList.Count} trigger words found in the page text", EditorStyles.miniLabel);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Expand all", EditorStyles.miniButtonLeft)) { for (int i = 0; i < showList.Length; i++) showList[i] = imageList[i] = true; }
        if (GUILayout.Button("Collapse all", EditorStyles.miniButtonMid)) { for (int i = 0; i < showList.Length; i++) showList[i] = infoList[i] = imageList[i] = false; }
        if (GUILayout.Button("Reload", EditorStyles.miniButtonRight)) { DisplayFacts(); GUIUtility.ExitGUI(); }
        EditorGUILayout.EndHorizontal();

        if (factList.Count < 1)
        {
            EditorGUILayout.HelpBox("No trigger words found. Mark words with <color=red>word</color=red> in the page text.", MessageType.Info);
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int i = 0; i < factList.Count; i++)
        {
            ShowFact(factList[i], i);
        }
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(4);
        GUI.backgroundColor = new Color(0.55f, 0.9f, 0.55f);
        if (GUILayout.Button($"Upload Facts for {BOOKNAME}", GUILayout.Height(34))) // when making a new fact make sure trigger word is the same as the key
        {
            Upload();
            GUIUtility.ExitGUI();
        }
        GUI.backgroundColor = Color.white;
    }
}
