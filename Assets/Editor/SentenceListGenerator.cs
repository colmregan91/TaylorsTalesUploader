using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SentenceListGenerator : TextPageUploader
{
    private Vector2 scrollPos;
    public override void OnGUI()
    {
        DisplayLanguageButtons(() => PageTextData = JsonUtility.FromJson<PageTextList>(File.ReadAllText(TextFilePath)));

        if (PageTextData == null) return;

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(Screen.width / 1.3f), GUILayout.Height(Screen.height / 1.4f));
        var areaStyle = new GUIStyle(GUI.skin.textArea);
        areaStyle.normal.textColor = Color.white;
        areaStyle.richText = true;
        areaStyle.wordWrap = true;
        areaStyle.alignment = (int)TextAlignment.Left;
        foreach (var page in PageTextData.pageTexts)
        {

            GUILayout.Label(page.Texts[(int)CurrentLanguageIndex], areaStyle);
            areaStyle.CalcHeight(new GUIContent(page.Texts[(int)CurrentLanguageIndex]), 9);
        }

        EditorGUILayout.EndScrollView();
    }
}
