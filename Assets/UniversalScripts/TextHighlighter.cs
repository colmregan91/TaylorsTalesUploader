using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TextHighlighter : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI TMPro;
    public int currentWord = 0;
    private int lastWord = 0;
    private int lastWord2 = 0;
    public int currentSentence = 0;
    private float lerpTime;
    private float resetlerpTime;
    private float highlightSpeed = 2f;
    private WaitForSeconds HalfSecondWait = new WaitForSeconds(0.3f);

    public bool isSentenceFin;

    [SerializeField]
    public Dictionary<int, int> wordsInEachSentence = new Dictionary<int, int>();
    private int targetWord;
    bool isspec;
    bool isspec2;
    bool isspec3;
    List<int> reds = new List<int>();

    private IEnumerator Start()
    {
        while (TMPro.textInfo.wordCount == 0)
        {
            yield return null;
        }

        CountWordsinSentences();
    }

    private void Update()
    {
        Debug.Log(TMPro.textInfo.wordCount);
    }
    public void CountWordsinSentences()
    {
        reds.Clear();
        wordsInEachSentence.Clear();
        var sentence = 0;
        var words = 0;
        Debug.Log(TMPro.textInfo.wordCount);
        for (int i = 0; i < TMPro.textInfo.wordCount; i++)
        {
            var lastletter = TMPro.textInfo.wordInfo[i].lastCharacterIndex + 1;
            var lastChar = TMPro.textInfo.characterInfo[lastletter].character;

            if (IsWordRed(TMPro, i))
            {
                reds.Add(i);
            }

            if (lastChar.Equals('.'))
            {

                words++;

                wordsInEachSentence.Add(sentence, words);
                sentence++;
                words = 0;

                //   break;
            }
            else
            {
                words++;
            }

        }

    }
    public static bool IsWordRed(TextMeshProUGUI _text, int wordIndex)
    {
        TMP_WordInfo info = _text.textInfo.wordInfo[wordIndex];
        int charIndex = info.firstCharacterIndex + 1;
        int meshIndex = _text.textInfo.characterInfo[charIndex].materialReferenceIndex;
        int vertexIndex = _text.textInfo.characterInfo[charIndex].vertexIndex;
        Color32[] vertexColors = _text.textInfo.meshInfo[meshIndex].colors32;

        return vertexColors[vertexIndex + 1] == Color.red;
    }
    private bool isred(int word)
    {
        return reds.Contains(word);
    }
    public void stopHighlightCoro(bool reset)
    {
        StopAllCoroutines();
        if (reset)
        {
            resetlerpTime = 0;
            dReset();
        }

    }

    public void highlightSentenceWords()
    {
        StartCoroutine(highlightCoro());
    }
    IEnumerator highlightCoro()
    {
        currentWord = getStartOfSentenceWord();
        var numberWrdsThisSent = wordsInEachSentence[currentSentence];
        targetWord = currentWord + numberWrdsThisSent;

        isSentenceFin = false;
        while (currentWord <= targetWord)
        {
            lerpTime = 0;
            lastWord = currentWord - 1;
            if (lastWord >= 0)
            {
                isspec = isred(lastWord);
            }

            lastWord2 = currentWord - 2;
            if (lastWord2 >= 0)
            {
                isspec2 = isred(lastWord2);
            }
            while (true)
            {


                if (currentWord == targetWord)
                {
                    LerplastWord(currentWord);

                }
                else
                {
                    LerpWord(currentWord);
                }


                if (lerpTime > 0.6f)
                {
                    break;
                }

                yield return null;
            }

            currentWord++;

        }

        currentSentence++;
        isSentenceFin = true;
        yield break;
    }
    void LerplastWord(int currentWrd)
    {
        lerpTime += highlightSpeed * Time.deltaTime;
       LerpWordColor(TMPro, currentWrd - 1, Color.black, lerpTime, true, false, isspec);
    }

    private void dReset()
    {
        isspec3 = isred(currentWord);
        LerpWordColor(TMPro, currentWord, Color.black, 1, true, false, isspec3);
        LerpWordColor(TMPro, lastWord, Color.black, 1, true, false, isspec);
        LerpWordColor(TMPro, lastWord2, Color.black, 1, true, false, isspec2);

    }


    public static void LerpWordColor(TextMeshProUGUI _text, int wordIndex, Color whatcolor, float colorLerpTime, bool isReversing = false, bool isFact = false, bool isSpec = false)
    {
        TMP_WordInfo info = _text.textInfo.wordInfo[wordIndex];

        if (isSpec && isReversing && !isFact)
        {
            whatcolor = Color.red;
        }

        for (int i = 0; i < info.characterCount; ++i)
        {
            int charIndex = info.firstCharacterIndex + i;
            int meshIndex = _text.textInfo.characterInfo[charIndex].materialReferenceIndex;
            int vertexIndex = _text.textInfo.characterInfo[charIndex].vertexIndex;

            Color32[] vertexColors = _text.textInfo.meshInfo[meshIndex].colors32;

            vertexColors[vertexIndex + 0] = Color.Lerp(vertexColors[vertexIndex + 0], whatcolor, colorLerpTime);
            vertexColors[vertexIndex + 1] = Color.Lerp(vertexColors[vertexIndex + 1], whatcolor, colorLerpTime);
            vertexColors[vertexIndex + 2] = Color.Lerp(vertexColors[vertexIndex + 2], whatcolor, colorLerpTime);
            vertexColors[vertexIndex + 3] = Color.Lerp(vertexColors[vertexIndex + 3], whatcolor, colorLerpTime);
        }

        _text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    void LerpWord(int currentWrd)
    {


        lerpTime += highlightSpeed * Time.deltaTime;

        LerpWordColor(TMPro, currentWrd, Color.blue, lerpTime);

        if (lastWord >= 0 && lerpTime > 0.3f)
        {

           LerpWordColor(TMPro, lastWord, Color.black, lerpTime, true, false, isspec);
        }

        if (lastWord2 >= 0)
        {
           LerpWordColor(TMPro, lastWord2, Color.black, lerpTime, true, false, isspec2);
        }

    }

    private int getStartOfSentenceWord()
    {
        var f = 0;
        foreach (var wrd in wordsInEachSentence)
        {

            if (currentSentence == wrd.Key)
            {
                break;
            }
            f += wrd.Value;
        }
        return currentSentence == 0 ? 0 : f;
    }

}
