using UnityEngine;
using UnityEditor;

public class SentenceListGenerator : TextPageUploader
{
    private Vector2 scrollPos;

    private void OnFocus()
    {
        if (TextFilePath != null) LoadBookText(); // pick up edits saved in the Page Text window
    }

    public override void OnGUI()
    {
        DrawHeader("Sentence List", "Every saved page of the book, in reading order");
        if (PageTextData == null) return;

        DisplayLanguageButtons();

        var headerStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 12 };
        var areaStyle = new GUIStyle(EditorStyles.helpBox) { richText = true, wordWrap = true, fontSize = 13, alignment = TextAnchor.UpperLeft, padding = new RectOffset(10, 10, 8, 8) };

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var page in PageTextData.pageTexts)
        {
            var text = page.Texts != null && page.Texts.Length > (int)CurrentLanguageIndex ? page.Texts[(int)CurrentLanguageIndex] : null;
            EditorGUILayout.LabelField($"Page {page.pageNumber}", headerStyle);
            GUILayout.Label(string.IsNullOrEmpty(text) ? $"<i>No {CurrentLanguageIndex} text yet</i>" : ToPreview(text), areaStyle);
            EditorGUILayout.Space(4);
        }
        EditorGUILayout.EndScrollView();
    }
}
