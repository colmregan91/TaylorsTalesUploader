using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PageTextUploader : BaseEditorWindow
{
    public int Number;
    private Page PageData;

    private const string pageFileName = "JSONPage_";


    private void OnGUI()
    {
        Number = EditorGUILayout.IntField("Enter Page number :", Number);


        if (Number == 0) return;
        string ThisPageFileName = $"{pageFileName}{Number}";

        var TextFilePath = $"{ASSETPATH}/Page_{Number}/{ThisPageFileName}{jsonExtension}";
        var dirPath = $"{ASSETPATH}/Page_{Number}";
        bool dirExists = Directory.Exists(dirPath);

        if (!dirExists)
        {
            EditorGUILayout.LabelField($"Page {Number} has not been created yet");
            return;
        }
        bool fileExists = File.Exists(TextFilePath);
        string buttonInfo = fileExists  ? $"Load Text for page {Number}" : $"Create Text for page {Number}";
        if (GUILayout.Button(buttonInfo))
        {
            EditorGUILayout.EndHorizontal();
            if (fileExists)
            {
                var text = File.ReadAllText(TextFilePath);
                PageData = JsonUtility.FromJson<Page>(text);
            }
            else
            {
                PageData = new Page();
                PageData.pageNumber = Number;
                PageData.Texts = new string[LANGUAGESAVAILABLE];
            }
            

        }
        if (PageData == null) return;
        DisplayLanguageButtons();

        var areaStyle = new GUIStyle(GUI.skin.textArea);
        areaStyle.normal.textColor = Color.white;
        areaStyle.richText = true;
        areaStyle.wordWrap = true;
        

        PageData.Texts[(int)CurrentLanguageIndex] = EditorGUILayout.TextArea(PageData.Texts[(int)CurrentLanguageIndex], areaStyle);
        areaStyle.CalcHeight(new GUIContent(PageData.Texts[(int)CurrentLanguageIndex]), 9);


        if (GUILayout.Button($"Upload text for page {Number}"))
        {
            if (fileExists)
            {
                DeleteFile(TextFilePath);
            }

            var pageJSON = JsonUtility.ToJson(PageData, true);
            File.WriteAllText(TextFilePath, pageJSON);

        }

    }


}


