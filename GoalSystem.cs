using System.Collections.Generic;

public interface Collectable
{
    public void Collect() { }
}

public class Objective { }

public class GoalList
{
    private readonly List<Objective> _list;

    public GoalList()
    {
        _list = new();
    }

    public GoalList(List<Objective> list)
    {
        _list = new(list);
    }

    public List<Objective> Get()
    {
        return _list;
    }
}