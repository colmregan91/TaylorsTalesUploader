using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WordListGenerator : BaseEditorWindow
{
    string[] wordsForLanguage;
    private Vector2 scrollPos;

    private string[] WordsForLanguage(Languages languageIndex)
    {
        List<string> savedWords = new List<string>();
        var languagedata = GetLanguages(languageIndex);

        if (languagedata == null)
        {
            return null;
        }

        for (int i = 0; i < languagedata.items.Length; i++)
        {
            var words = languagedata.items[i].value.Split(' ', '\n', '.', '“', '”', ',', '?');

            for (int j = 0; j < words.Length; j++)
            {
                var newword = string.Empty;
                if (words[j].Contains("<color=red>"))
                {
                    var wordwithExtra = words[j];
                    newword = wordwithExtra.Replace("<color=red>", "").Replace("</color=red>", "");
                }
                else
                {
                    newword = words[j];
                }

                var wordToLower = newword.ToLower();
                if (!savedWords.Contains(wordToLower))
                {

                    savedWords.Add(wordToLower);
                }
            }
        }



        return savedWords.ToArray();
    }

    private void OnGUI()
    {
        DisplayLanguageButtons(() => wordsForLanguage = WordsForLanguage(CurrentLanguageIndex));

        if (wordsForLanguage != null)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width / 1.3f), GUILayout.Height(Screen.height / 1.4f));

            var areaStyle = new GUIStyle(GUI.skin.textArea);
            areaStyle.wordWrap = true;
            areaStyle.fontSize = 15;
            int wordsPerRow = Mathf.CeilToInt((float)wordsForLanguage.Length / 6);

            for (int rowIndex = 0; rowIndex < wordsPerRow; rowIndex++)
            {
                EditorGUILayout.BeginHorizontal();

                for (int columnIndex = 0; columnIndex < 6; columnIndex++)
                {
                    int wordIndex = rowIndex + columnIndex * wordsPerRow;

                    if (wordIndex < wordsForLanguage.Length)
                    {
                        GUILayout.Label(wordsForLanguage[wordIndex], areaStyle, GUILayout.Width(Screen.width / 6));
                    }
                    else
                    {
                        GUILayout.Label("", areaStyle, GUILayout.Width(Screen.width / 6));
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(5);
            }

            EditorGUILayout.EndScrollView();
        }


    }
}

