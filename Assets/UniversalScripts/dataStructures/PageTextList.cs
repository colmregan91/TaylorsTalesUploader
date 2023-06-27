using System.Collections.Generic;

[System.Serializable]
public class PageTextList
{
    public List<Page> pageTexts = new List<Page>();
    public PageTextList(List<Page> _texts)
    {
        pageTexts = _texts;
    }
}
