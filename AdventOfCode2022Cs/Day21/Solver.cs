namespace AdventOfCode2022Cs.Day21;

public static class Solver
{

    public static void Solve()
    {

        //Solve("Day21/input.txt");

        Solve2("Day21/test.txt");
    }

    private static void Solve(string filename)
    {
        var monkeys = LoadMonkeys(filename);

        ProcessMonkeys(monkeys);

        Console.WriteLine($"Root: {monkeys["root"].Value(monkeys).Value}");
    }

    private static void Solve2(string filename)
    {
        var monkeys = LoadMonkeys(filename, "humn");

        ProcessMonkeys(monkeys, "humn");

        Console.WriteLine($"Root: {monkeys["root"].Value(monkeys).Value}");
    }

    private static void ProcessMonkeys(Dictionary<string, Monkey> monkeys, string humanId = "")
    {
        var monkeysToProcess = new List<Monkey>(monkeys.Values);

        while (monkeysToProcess.Any())
        {
            for (int i = 0; i < monkeysToProcess.Count; i++)
            {
                var v = monkeysToProcess[i].Value(monkeys);

                if (v.HasValue)
                {
                    monkeysToProcess.RemoveAt(i);
                    i--;
                }
            }

            if (!string.IsNullOrWhiteSpace(humanId))
            {
                var root = monkeys["root"];

                var a = root.A(monkeys);
                var b = root.B(monkeys);

                var av = a.Value(monkeys);
                var bv = b.Value(monkeys);

                if (av.HasValue && bv.HasValue)
                {
                    continue;
                }

                if (bv.HasValue || av.HasValue)
                {
                    var c = av.HasValue ? av.Value : bv.Value;
                    var unsolved = av.HasValue ? b : a;

                    var backwardsMonkeys = SolveBackwards(monkeys, unsolved, c);

                    var backwardsHuman = backwardsMonkeys[humanId];
                    monkeys[humanId].SetHumanValue(backwardsHuman.Value(backwardsMonkeys).Value);
                }
            }
        }
    }

    private static Dictionary<string, Monkey> SolveBackwards(Dictionary<string, Monkey> oldMonkeys, Monkey unsolved, long overrideVal)
    {
        var backwardsMonkeys = oldMonkeys.Values.Select(x =>
            {
                if (x.Id == unsolved.Id)
                {
                    return new Monkey(x, overrideVal);
                }
                    
                return new Monkey(x);
            })
            .ToDictionary(x => x.Id, x => x);

        var monkeysToProcess = new List<Monkey>(backwardsMonkeys.Values);

        while (monkeysToProcess.Any())
        {
            for (int i = 0; i < monkeysToProcess.Count; i++)
            {
                var v = monkeysToProcess[i].Value(backwardsMonkeys);

                if (v.HasValue)
                {
                    monkeysToProcess.RemoveAt(i);
                    i--;
                }
            }

            if (monkeysToProcess.All(x => x.IsHuman || x.IsRoot))
            {
                var h = backwardsMonkeys["humn"];

                h.Value(backwardsMonkeys);

                break;
            }

            Console.WriteLine($"Did a backwards pass");

            //var unsolved = monkeysToProcess.Where(x=>x.IsntFinished)
        }

        return backwardsMonkeys;
    }

    private static Dictionary<string, Monkey> LoadMonkeys(string filename, string humanMonkey = "")
    {
        var lines = File.ReadAllLines(filename)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => new Monkey(x, humanMonkey))
            .ToDictionary(x => x.Id, x => x);

        return lines;
    }

    class Monkey
    {

        public bool IsHuman { get; }
        public string Id { get; }
        public bool IsRoot => Id == "root";

        private readonly (string a, string b)? _dependsOn;

        private readonly string _operator;

        private long? _fixedValue;

        public bool IsntFinished => !_fixedValue.HasValue;

        public bool DependsOnHuman => (_dependsOn.HasValue && (_dependsOn.Value.a == "humn" || _dependsOn.Value.b == "humn"));

        public string NonHumanDependsOn => _dependsOn.HasValue ? (_dependsOn.Value.a == "humn" ? _dependsOn.Value.b : _dependsOn.Value.a) : string.Empty;

        public bool IsBackwards { get; }

        public Monkey(Monkey m, long? overrideValue = null)
        {
            IsBackwards = true;

            Id = m.Id;
            IsHuman = m.IsHuman;
            _fixedValue = !IsHuman ? m._fixedValue : null;
            _operator = m._operator;
            _dependsOn = m._dependsOn;

            if (overrideValue.HasValue)
            {
                _fixedValue = overrideValue.Value;
            }
        }

        public Monkey(string line, string humanMonkey)
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            Id = parts[0].Replace(":", string.Empty);
            IsHuman = Id == humanMonkey;

            if (parts.Length == 2)
            {
                _fixedValue = Convert.ToInt64(parts[1]);
                _dependsOn = null;
                _operator = string.Empty;
            }
            else if (parts.Length == 4)
            {
                _dependsOn = (parts[1], parts[3]);
                _operator = parts[2];
            }
            else
            {
                throw new InvalidOperationException($"Unable to parse {line}");
            }
        }

        public Monkey A(Dictionary<string, Monkey> monkeys) => monkeys[_dependsOn.Value.a];
        public Monkey B(Dictionary<string, Monkey> monkeys) => monkeys[_dependsOn.Value.b];

        public long? Value(Dictionary<string, Monkey> allMonkeys)
        {
            if (IsHuman && !IsBackwards)
            {
                return _fixedValue;
            }

            if (_fixedValue.HasValue)
            {
                return _fixedValue.Value;
            }

            _fixedValue = IsBackwards ? CalculateValueBackwards(allMonkeys) : CalculateValue(allMonkeys);
            return _fixedValue;
        }

        private long? CalculateValueBackwards(Dictionary<string, Monkey> allMonkeys)
        {
            if (_fixedValue.HasValue)
            {
                return _fixedValue.Value;
            }

            //foreach (var other in allMonkeys.Values)
            //{
            //    var dv = other._fixedValue.Value;

            //    if (dv.HasValue)
            //    {
            //        _fixedValue = InverseValue(dv.Value, p);
            //        return _fixedValue;
            //    }
            //}

            //if (IsHuman)
            //{
            //    foreach (var other in allMonkeys.Values)
            //    {
            //        var dv = other.GetDependentValue(Id);

            //        if (dv.HasValue)
            //        {
            //            foreach (var other2 in allMonkeys.Values)
            //            {
            //                var s2 = other2.GetDependentValue(other.Id);

            //                if (s2.HasValue)
            //                {
            //                    _fixedValue = other.InverseValue(s2.Value, other._fixedValue.Value);
            //                    return _fixedValue;
            //                }
            //            }

            //        }
            //    }

            //    return _fixedValue;
            //}

            //var av = A(allMonkeys).CalculateValueBackwards(allMonkeys);
            //var bv = B(allMonkeys).CalculateValueBackwards(allMonkeys);

            //if (av.HasValue || bv.HasValue)
            //{
            //    var p = av.HasValue ? av.Value : bv.Value;

            //    foreach (var other in allMonkeys.Values)
            //    {
            //        var dv = other.GetDependentValue(Id);

            //        if (dv.HasValue)
            //        {
            //            _fixedValue = InverseValue(dv.Value, p);
            //            return _fixedValue;
            //        }
            //    }
            //}

            return null;
        }

        private long? GetDependentValue(string dependsOn)
        {
            if (_dependsOn.HasValue && (_dependsOn.Value.a == dependsOn || _dependsOn.Value.b == dependsOn))
            {
                return _fixedValue;
            }

            return null;
        }

        private long? CalculateValue(Dictionary<string, Monkey> allMonkeys)
        {
            if (_dependsOn.HasValue)
            {
                var a = A(allMonkeys);
                var b = B(allMonkeys);

                var av = a.Value(allMonkeys);
                var bv = b.Value(allMonkeys);

                if (av.HasValue && bv.HasValue)
                {
                    switch (_operator)
                    {
                        case "+": return av.Value + bv.Value;
                        case "/": return av.Value / bv.Value;
                        case "-": return av.Value - bv.Value;
                        case "*": return av.Value * bv.Value;
                        default: throw new NotImplementedException($"Unable to operate with {_operator}");
                    }
                }

                return null;
            }

            throw new NotImplementedException();
        }

        public override string ToString()
        {
            var sum = _dependsOn.HasValue ? $"{_dependsOn.Value.a} {_operator} {_dependsOn.Value.b} " : string.Empty;

            var h = DependsOnHuman ? "* " : string.Empty;

            return $"{h}{Id}: {sum}{(_fixedValue.HasValue ? _fixedValue.Value : "")}";
        }

        public void SetHumanValue(long hv)
        {
            if (IsHuman)
            {
                _fixedValue = hv;
            }
            else
            {
                throw new InvalidOperationException($"Tried to set human value on normal monkey");
            }
        }

        public long InverseValue(long target, long other)
        {
            switch (_operator)
            {
                case "+": return target - other;
                case "/": return target * other;
                case "-": return target + other;
                case "*": return target / other;
                default: throw new NotImplementedException($"Unable to operate with {_operator}");
            }
            throw new NotImplementedException();
        }
    }
}