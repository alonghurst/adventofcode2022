namespace AdventOfCode2022Cs.Day7;

public abstract class Entry
{
    protected Entry(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public abstract int Size { get; }

    public override string ToString() => Name;
}

public class File : Entry
{
    public File(string name, int size) : base(name)
    {
        Size = size;
    }

    public override int Size { get; }
}

public class Directory : Entry
{
    public Directory(string name) : base(name)
    {
    }

    public HashSet<Entry> Entries { get; } = new();

    public override int Size => Entries.Sum(x => x.Size);
}