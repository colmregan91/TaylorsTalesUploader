using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TextPageUploader : BaseEditorWindow
{

    public int Number;
    public PageTextList PageTextData;

    private const string pageFileName = "JSONPageData";

    private Page curPage;
    private string TextFilePath;

    public override void OnEnable()
    {
        base.OnEnable();
        TextFilePath = $"{ASSETPATH}/{pageFileName}{jsonExtension}";
        bool dirExists = File.Exists(TextFilePath);

        if (!dirExists)
        {
            PageTextData = new PageTextList(new List<Page>());
            var pageJSON = JsonUtility.ToJson(PageTextData, true);
            File.WriteAllText(TextFilePath, pageJSON);

        }

    }

    private bool contains(int number)
    {
        for (int i = 0; i < PageTextData.pageTexts.Count; i++)
        {
            if (PageTextData.pageTexts[i].pageNumber == number) return true;
        }

        return false;
    }
    private void OnGUI()
    {
        Number = EditorGUILayout.IntField("Enter Page number :", Number);


        if (Number == 0) return;

        if (GUILayout.Button("Load Book Text"))
        {
            var text = File.ReadAllText(TextFilePath);
            PageTextData = JsonUtility.FromJson<PageTextList>(text);

            if (PageTextData.pageTexts == null)
            {
                PageTextData.pageTexts = new List<Page>();
            }

            if (!contains(Number))
            {
                curPage = new Page();
                curPage.pageNumber = Number;
                curPage.Texts = new string[LANGUAGESAVAILABLE];
                PageTextData.pageTexts.Add(curPage);
            }
            else
            {
                curPage = PageTextData.pageTexts[Number-1];
            }


        }

        if (curPage == null || PageTextData == null) return;

        DisplayLanguageButtons();

        var areaStyle = new GUIStyle(GUI.skin.textArea);
        areaStyle.normal.textColor = Color.white;
        areaStyle.richText = true;
        areaStyle.wordWrap = true;


        curPage.Texts[(int)CurrentLanguageIndex] = EditorGUILayout.TextArea(curPage.Texts[(int)CurrentLanguageIndex], areaStyle);
        areaStyle.CalcHeight(new GUIContent(curPage.Texts[(int)CurrentLanguageIndex]), 9);

        if (GUILayout.Button($"Upload {CurrentLanguageIndex.ToString()} text for page {Number}"))
        {

            DeleteFile(TextFilePath);

            var pageJSON = JsonUtility.ToJson(PageTextData, true);
            File.WriteAllText(TextFilePath, pageJSON);

        }
    }
}
