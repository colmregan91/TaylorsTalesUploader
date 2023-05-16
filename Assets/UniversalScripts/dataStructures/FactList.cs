using System.Collections.Generic;

[System.Serializable]
public class FactList
{
    public List<Fact> Facts = new List<Fact>();

    public FactList(List<Fact> _facts)
    {
        Facts = _facts;
    }
}
