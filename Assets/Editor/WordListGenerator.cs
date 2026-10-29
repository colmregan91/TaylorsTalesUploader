using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class WordListGenerator : BaseEditorWindow
{
    private const int Columns = 6;
    private static readonly char[] Separators = { ' ', '\n', '.', '“', '”', '"', ',', '?', '!', ';', ':', '(', ')' };

    string[] wordsForLanguage;
    HashSet<string> triggerWords = new HashSet<string>();
    private Vector2 scrollPos;
    private string filter = "";

    public override void OnEnable()
    {
        base.OnEnable();
        wordsForLanguage = WordsForLanguage(CurrentLanguageIndex);
    }

    private string[] WordsForLanguage(Languages languageIndex)
    {
        var savedWords = new HashSet<string>();
        triggerWords.Clear();
        var languagedata = GetLanguages(languageIndex);

        if (languagedata == null)
        {
            return null;
        }

        for (int i = 0; i < languagedata.items.Length; i++)
        {
            var words = languagedata.items[i].value.Replace("\\n", "\n").Split(Separators, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                bool isTrigger = word.Contains("<color=red>");
                var wordToLower = word.Replace("<color=red>", "").Replace("</color=red>", "").Trim('\'', '’', '‘').ToLower();
                if (wordToLower.Length == 0) continue;

                savedWords.Add(wordToLower);
                if (isTrigger) triggerWords.Add(wordToLower);
            }
        }

        return savedWords.OrderBy(w => w).ToArray();
    }

    private void OnGUI()
    {
        DrawHeader("Word List", "Every unique word in the book, fact trigger words highlighted");
        DisplayLanguageButtons(() => wordsForLanguage = WordsForLanguage(CurrentLanguageIndex));

        if (wordsForLanguage == null)
        {
            EditorGUILayout.HelpBox($"No source text found at {LANGUAGESPATH}/{CurrentLanguageIndex}.txt", MessageType.Info);
            return;
        }

        EditorGUILayout.BeginHorizontal();
        filter = EditorGUILayout.TextField("Filter", filter);
        GUILayout.Label($"{wordsForLanguage.Length} words  •  {triggerWords.Count} triggers", EditorStyles.miniLabel, GUILayout.Width(160));
        EditorGUILayout.EndHorizontal();

        var shown = string.IsNullOrEmpty(filter) ? wordsForLanguage : wordsForLanguage.Where(w => w.Contains(filter.ToLower())).ToArray();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        var wordStyle = new GUIStyle(EditorStyles.helpBox) { fontSize = 13, alignment = TextAnchor.MiddleLeft };
        var triggerStyle = new GUIStyle(wordStyle) { fontStyle = FontStyle.Bold, normal = { textColor = new Color(1f, 0.42f, 0.3f) } };
        float columnWidth = (position.width - 30) / Columns;
        int rows = Mathf.CeilToInt((float)shown.Length / Columns);

        for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int columnIndex = 0; columnIndex < Columns; columnIndex++)
            {
                int wordIndex = rowIndex + columnIndex * rows; // fill down each column so the list reads alphabetically

                if (wordIndex < shown.Length)
                {
                    var word = shown[wordIndex];
                    GUILayout.Label(word, triggerWords.Contains(word) ? triggerStyle : wordStyle, GUILayout.Width(columnWidth));
                }
                else
                {
                    GUILayout.Label("", GUIStyle.none, GUILayout.Width(columnWidth));
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }
}
