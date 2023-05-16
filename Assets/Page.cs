using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Page
{
    public int pageNumber;
    public string[] Texts;
}

public class Book
{
//    public Scene TitleScene;
    public List<Page> Pages;
}


[System.Serializable]
public class Fact
{
    public string[] TriggerWords;
    public string[] FactInfo;
    public string imagesBundle;
}


[System.Serializable]
public class FactList
{
    public List<Fact> Facts = new List<Fact>();

    public FactList(List<Fact> _facts)
    {
        Facts = _facts;
    }
}
