using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class TextPageUploader : BaseEditorWindow
{
    public int Number = 1;
    public PageTextList PageTextData;

    protected const string pageFileName = "JSONPageData";
    protected const string TriggerOpen = "<color=red>";
    protected const string TriggerClose = "</color=red>";
    private static readonly Regex TriggerRegex = new Regex(@"<color=red>(.*?)</color=red>");

    protected Page curPage;
    protected string TextFilePath;
    private string wordToHighlight = "";
    private Vector2 editScroll, previewScroll;
    private bool dirty;

    public override void OnEnable()
    {
        base.OnEnable();
        TextFilePath = $"{PAGESPATH}/{pageFileName}{jsonExtension}";

        if (!File.Exists(TextFilePath))
        {
            PageTextData = new PageTextList(new List<Page>());
            File.WriteAllText(TextFilePath, JsonUtility.ToJson(PageTextData, true));
        }
        LoadBookText();
    }

    protected void LoadBookText()
    {
        PageTextData = JsonUtility.FromJson<PageTextList>(File.ReadAllText(TextFilePath));
        if (PageTextData.pageTexts == null) PageTextData.pageTexts = new List<Page>();
        SelectPage(Number);
        dirty = false;
    }

    protected Page FindPage(int number)
    {
        return PageTextData.pageTexts.FirstOrDefault(p => p.pageNumber == number);
    }

    public static void OpenAtPage(int number)
    {
        var window = GetWindow<TextPageUploader>("Page Text");
        window.SelectPage(number);
        window.Repaint();
    }

    private void SelectPage(int number)
    {
        Number = Mathf.Max(1, number);
        curPage = FindPage(Number);
        if (curPage == null)
        {
            curPage = new Page { pageNumber = Number, Texts = new string[LANGUAGESAVAILABLE] };
        }
        else if (curPage.Texts == null || curPage.Texts.Length < LANGUAGESAVAILABLE)
        {
            var texts = new string[LANGUAGESAVAILABLE];
            curPage.Texts?.CopyTo(texts, 0);
            curPage.Texts = texts;
        }
        GUI.FocusControl(null);
    }

    private void Save()
    {
        if (FindPage(curPage.pageNumber) == null) PageTextData.pageTexts.Add(curPage);
        PageTextData.pageTexts = PageTextData.pageTexts.OrderBy(p => p.pageNumber).ToList();

        File.WriteAllText(TextFilePath, JsonUtility.ToJson(PageTextData, true));
        dirty = false;
        ShowNotification(new GUIContent($"Saved page {Number} ({CurrentLanguageIndex})"));
    }

    private void ImportFromSource()
    {
        var source = GetLanguages(CurrentLanguageIndex);
        if (source == null || source.items.Length < Number)
        {
            EditorUtility.DisplayDialog("Import text", $"No source text for page {Number} in {LANGUAGESPATH}/{CurrentLanguageIndex}.txt", "OK");
            return;
        }
        curPage.Texts[(int)CurrentLanguageIndex] = source.items[Number - 1].value;
        dirty = true;
        GUI.FocusControl(null);
    }

    private void HighlightWord(string word)
    {
        var text = curPage.Texts[(int)CurrentLanguageIndex] ?? "";
        // wrap the first occurrence that isn't already a trigger word
        var match = Regex.Match(text, $@"(?<!<color=red>)\b{Regex.Escape(word)}\b(?!</color=red>)", RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            ShowNotification(new GUIContent($"'{word}' not found"));
            return;
        }
        curPage.Texts[(int)CurrentLanguageIndex] = text.Substring(0, match.Index) + TriggerOpen + match.Value + TriggerClose + text.Substring(match.Index + match.Length);
        dirty = true;
        GUI.FocusControl(null);
    }

    protected static string ToPreview(string text)
    {
        // the reader uses </color=red>, IMGUI rich text only understands </color>
        return (text ?? "").Replace(TriggerOpen, "<color=#FF6A4D><b>").Replace(TriggerClose, "</b></color>");
    }

    public virtual void OnGUI()
    {
        DrawHeader("Page Text", "Write each page's paragraphs per language and mark fact trigger words");
        DrawOutputInfo(PAGESPATH);
        if (PageTextData == null || curPage == null) return;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("◀", GUILayout.Width(28))) SelectPage(Number - 1);
        var entered = EditorGUILayout.IntField("Page", Number);
        if (entered != Number) SelectPage(entered);
        if (GUILayout.Button("▶", GUILayout.Width(28))) SelectPage(Number + 1);
        GUILayout.Label($"{PageTextData.pageTexts.Count} pages saved", EditorStyles.miniLabel, GUILayout.Width(100));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(4);
        DisplayLanguageButtons(null, l => !string.IsNullOrEmpty(curPage.Texts[(int)l]));

        int lang = (int)CurrentLanguageIndex;
        var areaStyle = new GUIStyle(EditorStyles.textArea) { wordWrap = true, fontSize = 13 };

        EditorGUILayout.LabelField("Source", EditorStyles.boldLabel);
        editScroll = EditorGUILayout.BeginScrollView(editScroll, GUILayout.MinHeight(140), GUILayout.MaxHeight(position.height * 0.3f));
        EditorGUI.BeginChangeCheck();
        curPage.Texts[lang] = EditorGUILayout.TextArea(curPage.Texts[lang] ?? "", areaStyle, GUILayout.ExpandHeight(true));
        if (EditorGUI.EndChangeCheck()) dirty = true;
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        wordToHighlight = EditorGUILayout.TextField("Mark trigger word", wordToHighlight);
        EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(wordToHighlight));
        if (GUILayout.Button("Mark", GUILayout.Width(60))) HighlightWord(wordToHighlight.Trim());
        EditorGUI.EndDisabledGroup();
        if (GUILayout.Button("Import from source text", GUILayout.Width(170))) ImportFromSource();
        EditorGUILayout.EndHorizontal();

        var triggers = TriggerRegex.Matches(curPage.Texts[lang] ?? "").Cast<Match>().Select(m => m.Groups[1].Value.ToLower()).Distinct().ToArray();
        EditorGUILayout.LabelField(triggers.Length == 0 ? "No fact trigger words on this page" : $"Fact trigger words: {string.Join(", ", triggers)}", EditorStyles.miniLabel);

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
        var previewStyle = new GUIStyle(EditorStyles.helpBox) { richText = true, wordWrap = true, fontSize = 13, padding = new RectOffset(10, 10, 8, 8) };
        previewScroll = EditorGUILayout.BeginScrollView(previewScroll, GUILayout.ExpandHeight(true));
        GUILayout.Label(ToPreview(curPage.Texts[lang]), previewStyle);
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(4);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Revert", GUILayout.Height(30), GUILayout.Width(80))) LoadBookText();
        GUI.backgroundColor = dirty ? new Color(0.55f, 0.9f, 0.55f) : Color.white;
        if (GUILayout.Button(dirty ? $"Save page {Number}  •  unsaved changes" : $"Save page {Number}", GUILayout.Height(30))) Save();
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
    }
}
