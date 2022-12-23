namespace AdventOfCode2022Cs.Day7;

public static class Solver
{
    public static void Solve()
    {
        var lines = System.IO.File.ReadAllLines($"Day7/input.txt");

        var entry = Solver.BuildFileSystem(lines);

        Solver.WriteEntries(entry);

        var directories = Solver.GetAllDirectories(entry).ToArray();

        var gs = directories.GroupBy(x => x)
            .Where(x => x.Count() > 1)
            .ToArray();

        var min = 100000;
        var answer = directories.Where(x => x.Size <= min)
            .Sum(x => x.Size);

        Console.WriteLine(answer.ToString());

        var total = 70000000;
        var required = 30000000;
        var used = entry.Size;
        var unused = (total - used);

        var toDelete = required - unused;

        var di = directories.Where(x => x.Size >= toDelete)
            .OrderBy(x => x.Size)
            .First();

        Console.WriteLine(di.ToString());
        Console.WriteLine(di.Size);
    }

    public static Entry BuildFileSystem(string[] lines)
    {
        var allDirectories = new Dictionary<string, Directory>();
        var cwd = new Stack<Directory>();

        Directory GetDirectory(string name)
        {
            var path = name;

            foreach (var c in cwd)
            {
                path = $"{c}/{path}";
            }

            if (!allDirectories.ContainsKey(path))
            {
                var directory = new Directory(name);
                allDirectories.Add(path, directory);
            }

            return allDirectories[path];
        }

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var splits = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (splits[0] == "$")
            {
                if (splits[1] == "cd")
                {
                    var dir = splits[2];

                    if (dir == "..")
                    {
                        cwd.Pop();
                    }
                    else
                    {
                        var directory = GetDirectory(dir);

                        cwd.Push(directory);
                    }
                }

                continue;
            }

            if (int.TryParse(splits[0], out var size))
            {
                var file = new File(splits[1], size);

                cwd.Peek().Entries.Add(file);
            }
            else if (splits[0] == "dir")
            {
                var dir = GetDirectory(splits[1]);

                cwd.Peek().Entries.Add(dir);
            }
        }

        return allDirectories["/"];
    }

    public static void WriteEntries(Entry entry, int depth = 0)
    {
        var prefix = new string(' ', depth);

        var suffix = entry is File f ? ($"(file, size={f.Size})") : "(dir)";

        Console.WriteLine($"{prefix}- {entry.Name} {suffix}");

        if (entry is Directory directory)
        {
            foreach (var directoryEntry in directory.Entries)
            {
                WriteEntries(directoryEntry, depth + 1);
            }
        }
    }

    public static IEnumerable<Directory> GetAllDirectories(Entry entry)
    {
        if (entry is Directory directory)
        {
            yield return directory;

            foreach (var directoryEntry in directory.Entries)
            {
                var subs = GetAllDirectories(directoryEntry).ToArray();

                foreach (var sub in subs)
                {
                    yield return sub;
                }
            }
        }
    }
}