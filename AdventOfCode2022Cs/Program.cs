using AdventOfCode2022Cs.Day7;
using File = System.IO.File;

var lines = File.ReadAllLines($"Day7/input.txt");

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