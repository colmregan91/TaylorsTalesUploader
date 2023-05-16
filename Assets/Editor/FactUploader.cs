using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class FactUploader : BaseEditorWindow
// Start is called before the first frame update
{
    bool[] showList;
    bool[] infoList;
    bool[] imageList;
    bool isShowingList;
    List<Fact> factList = new List<Fact>();
    int shownIndex;
    Vector2 scrollPos;

    public override void OnEnable()
    {
        base.OnEnable();
        DisplayFacts();

    }

    void ShowFact(Fact fact, int factNumber)
    {
        
        showList[factNumber] = EditorGUILayout.Foldout(showList[factNumber], $"Fact {factNumber + 1} - {fact.TriggerWords[(int)Languages.English]}", true);
        if (showList[factNumber])
        {
            EditorGUI.indentLevel++;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Trigger Word English", fact.TriggerWords[(int)Languages.English]);
            EditorGUILayout.TextField("Trigger Word Irish", fact.TriggerWords[(int)Languages.Irish]);
            EditorGUILayout.TextField("Trigger Word French", fact.TriggerWords[(int)Languages.French]);
            EditorGUILayout.TextField("Trigger Word Spanish", fact.TriggerWords[(int)Languages.Spanish]);
            EditorGUI.EndDisabledGroup();

            infoList[factNumber] = EditorGUILayout.Foldout(infoList[factNumber], "FactInformation", true);

            if (infoList[factNumber])
            {

                var areaStyle = new GUIStyle(GUI.skin.textArea);
                areaStyle.wordWrap = true;
                GUILayout.Label("English");
                fact.FactInfo[(int)Languages.English] = EditorGUILayout.TextArea(fact.FactInfo[(int)Languages.English], areaStyle);
                areaStyle.CalcHeight(new GUIContent(fact.FactInfo[(int)Languages.English]), 9);
                GUILayout.Label("Irish");
                fact.FactInfo[(int)Languages.Irish] = EditorGUILayout.TextArea(fact.FactInfo[(int)Languages.Irish], areaStyle);
                areaStyle.CalcHeight(new GUIContent(fact.FactInfo[(int)Languages.Irish]), 9);
                GUILayout.Label("French");
                fact.FactInfo[(int)Languages.French] = EditorGUILayout.TextArea(fact.FactInfo[(int)Languages.French], areaStyle);
                areaStyle.CalcHeight(new GUIContent(fact.FactInfo[(int)Languages.French]), 9);
                GUILayout.Label("Spanish");
                fact.FactInfo[(int)Languages.Spanish] = EditorGUILayout.TextArea(fact.FactInfo[(int)Languages.Spanish], areaStyle);
                areaStyle.CalcHeight(new GUIContent(fact.FactInfo[(int)Languages.Spanish]), 9);
            }

            imageList[factNumber] = EditorGUILayout.Foldout(imageList[factNumber], "FactImages", true);

            if (imageList[factNumber])
            {
                string word = fact.TriggerWords[(int)Languages.English];
                var factImagesPath = $"{FACTIMAGES}/{word}";

                if (!Directory.Exists(factImagesPath))
                {
                    GUILayout.Label($"No images path for {word}");
                    return;
                }
                var images = Directory.GetFiles(factImagesPath);
                if (images.Length == 0)
                {
                    GUILayout.Label($"No images found in {word}");
                    return;
                }

                var spriteList = new Sprite[images.Length];

                var factImages = getJPGsorPNGs(factImagesPath);

                GUILayout.Label($"{word} image folder contains {factImages.Count()} image(s) that will be bundled");

            }
            EditorGUI.indentLevel--;


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
                    var word = specials[j];
                    var abb = word.Replace("<color=red>", "");
                    var TriggerWord = abb.Replace("</color=red>", "");
                    specialList.Add(TriggerWord);
                }
            }
        }

        return specialList.ToArray();
    }

    //private FactList CreateFactsForLanguage(string[] specialList, int index)
    //{
    //    var factData = GetFacts(index);

    //    for (int i = 0; i < specialList.Length; i++)
    //    {
    //        try
    //        {
    //            string word = specialList[i].ToLower();
    //            LocalisedItem factInfo = factData.items.First(T => T.key.Equals(word));
    //            Fact newFact = new Fact(word, factInfo.value);
    //            factList.Add(newFact);
    //        }
    //        catch (Exception localVariable)
    //        {
    //            Debug.LogWarning($"Need configuring for {TriggerWord}, {localVariable}");
    //        }

    //        CreateDirectoryIfDoesntExist($"{FACTIMAGES}/{TriggerWord}");
    //    }

    //}







    //    for (int i = 0; i < languagedata.items.Length; i++)
    //{
    //    var specials = languagedata.items[i].value.Split(' ', '\n', '.', '“', '”', ',', '?');
    //    for (int j = 0; j < specials.Length; j++)
    //    {

    //        if (specials[j].Contains("<color=red>"))
    //        {
    //            var word = specials[j];
    //            var abb = word.Replace("<color=red>", "");
    //            var TriggerWord = abb.Replace("</color=red>", "");
    //            //        Debug.Log(word);

    //            try
    //            {
    //                LocalisedItem factInfo = factData.items.First(T => T.key.Equals(TriggerWord.ToLower()));
    //                Fact newFact = new Fact(TriggerWord.ToLower(), factInfo.value);
    //                factList.Add(newFact);
    //            }
    //            catch (Exception localVariable)
    //            {
    //                Debug.LogWarning($"Need configuring for {TriggerWord}, {localVariable}");
    //            }



    //            CreateDirectoryIfDoesntExist($"{FACTIMAGES}/{TriggerWord}");


    //        }
    //    }
    private void DisplayFacts()
    {
        factList.Clear();

        var specialListEnglish = getSpecialWordsForLanguage(Languages.English);
        var SpecialListIrish = getSpecialWordsForLanguage(Languages.Irish);
     //   var FrenchSupported = getSpecialWordsForLanguage(Languages.French);
       // var SpanishSupported = getSpecialWordsForLanguage(Languages.Spanish);
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
            Debug.LogWarning($"{Languages.English.ToString()} not currently supported");
        }

        if (SpecialListIrish != null)
        {
            var factDataIrish = GetFacts(Languages.Irish);
            for (int i = 0; i < SpecialListIrish.Length; i++)
            {
                createFactForLanguage(SpecialListIrish[i], factDataIrish, (int)Languages.Irish, factList[i]);

            }
        }
        else
        {
            Debug.LogWarning($"{Languages.Irish.ToString()} not currently supported");
        }


        //var specialListFrench = getSpecialWordsForLanguage(Languages.French);
        //var specialListSpanish = getSpecialWordsForLanguage(Languages.Spanish);











        showList = new bool[factList.Count];
        imageList = new bool[factList.Count];
        infoList = new bool[factList.Count];

    }



    private void createFactForLanguage(string specWord, LocalisedData factlist, int langIndex, Fact newFact)
    {

        try
        {
            string word = specWord.ToLower();
            newFact.TriggerWords[langIndex] = word;
            string english = newFact.TriggerWords[(int)Languages.English];
            LocalisedItem factInfo = factlist.items.First(T => T.key.Equals(english));
        
            newFact.FactInfo[langIndex] = factInfo.value;


        }
        catch (Exception localVariable)
        {
            Debug.LogWarning($"Need configuring for {specWord}, {localVariable}");
        }

    }

    private LocalisedData GetFacts(Languages index)
    {
        string langName = getLanguageName(index);
     
        string fileName = $"{FACTPATH}/Facts{langName}.txt";

        if (File.Exists(fileName))
        {
            var text = File.ReadAllText(fileName);

            var loadeddata = JsonUtility.FromJson<LocalisedData>(text);
            return loadeddata;
        }
        else
        {
            return null;
        }
    }

    private void OnGUI()
    {
        scrollPos =
EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(1000), GUILayout.Height(500));


        isShowingList = factList.Count < 1;
        if (isShowingList) return;



        for (int i = 0; i < factList.Count; i++)
        {

            ShowFact(factList[i], i);

        }

        if (GUILayout.Button("Upload Facts for ChickenAndTheFox")) // when making a new fact make sure trigger word is the same as the key
        {
            AssetBundleUtils.ClearBundles();
            string FactPath = $"{ASSETPATH}/Facts";
            RecreateDirectory(FactPath);
            FactList factlist = new FactList(factList);

            for (int i = 0; i < factlist.Facts.Count; i++)
            {
                var imagePath = $"{FACTIMAGES}/{factlist.Facts[i].TriggerWords[(int)Languages.English]}";



                var files = Directory.GetFiles(imagePath).Where(T => Path.GetExtension(T).Equals(".jpg") || Path.GetExtension(T).Equals(".PNG"));


                if (files.Count() == 0)
                {
                    factlist.Facts[i].imagesBundle = string.Empty;
                    continue;
                }
                var bundleGuid = AssetBundleUtils.CreateNewBundle();

                factlist.Facts[i].imagesBundle = bundleGuid;
                foreach (var file in files)
                {
                    AssetBundleUtils.AddToBundle(file, bundleGuid);
                }
                Debug.Log($"{factlist.Facts[i].TriggerWords} {bundleGuid}");
            }



            AssetBundleUtils.BuildBundles(FactPath);
            var factPath = getFileswithoutMetaManifestFiles(FactPath);






            var factListJson = JsonUtility.ToJson(factlist, true);

            string target = $"{FactPath}/Facts.json";
            if (File.Exists(target))
            {
                File.Delete(target);
            }
            File.WriteAllText(target, factListJson);




        }
        EditorGUILayout.EndScrollView();
    }
}
