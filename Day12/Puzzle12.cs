

class Puzzle12 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day12/input.txt");
      //  lines = sample.Replace("\n", "").Split(['\r']);

        var test = NewMethod(".??..??...?##. 1,1,3");
        System.Console.WriteLine(test.IsValid("..#...#...###."));
        System.Console.WriteLine(test.IsValid("..#..#....###."));
        System.Console.WriteLine(test.IsValid(".#....#...###."));
        System.Console.WriteLine(test.IsValid(".#...#....###."));

        System.Console.WriteLine(test.IsValid(".##..##...###."));
        System.Console.WriteLine(test.IsValid(".#.#.##...###."));

        var total = 0;
        foreach (var line in lines)
        {
            ConditionMap condition = NewMethod(line);

            var combinations = condition.FindCombinations();

            System.Console.WriteLine($"{line} =>");
            foreach (var result in combinations)
            {
                System.Console.WriteLine($"  {result}");
                total++;
            }

            System.Console.WriteLine();
        }

        System.Console.WriteLine(total);
    }

    private static ConditionMap NewMethod(string line)
    {
        (string conditionMap, string p2) = line.Split(' ', 2);
        var conditionValues = p2.Split(',').Select(int.Parse);

        var condition = new ConditionMap(conditionMap, conditionValues.ToList());
        return condition;
    }

    record ConditionMap(string Map, IReadOnlyList<int> Values)
    {

        public IEnumerable<string> FindCombinations()
        {

            var results = new HashSet<string>();

            Recurse(Map, 0);

            return results;

            void Recurse(string candidate, int start)
            {
                for (int i = start; i < candidate.Length; i++)
                {
                    var c = candidate[i];

                    if (c == '?')
                    {
                        Recurse(candidate[..i] + '.' + candidate[(i + 1)..], i + 1);

                        Recurse(candidate[..i] + '#' + candidate[(i + 1)..], i + 1);

                    }
                }



                if (IsValid(candidate))
                {
                    results.Add(candidate);
                }


            }

        }

        public bool IsValid(string candidate)
        {

            var values = Values.GetEnumerator();
            var hasMoreValues = values.MoveNext();
            int digitsRemaining = 0;
            bool inValue = false;

            if (candidate.Length != Map.Length)
            {
                return false;
            }

            for (int i = 0; i < candidate.Length; i++)
            {
                var c = candidate[i];

                if (Map[i] != '?' && c != Map[i])
                {
                    return false;
                }

                if (c == '?')
                {
                    return false;
                }

                else if (c == '.')
                {
                    if (inValue)
                    {
                        if (digitsRemaining != 0)
                        {
                            return false;

                        }
                        hasMoreValues = values.MoveNext();
                        inValue = false;
                    }
                }

                else if (c == '#')
                {
                    if (!inValue)
                    {
                        inValue = true;
                        if (!hasMoreValues)
                        {
                            return false;
                        }

                        digitsRemaining = values.Current;
                    }
                    digitsRemaining--;
                }


            }
            if (digitsRemaining != 0)
            {
                return false;
            }
            if (inValue)
            {
                hasMoreValues = values.MoveNext();
            }
            if (hasMoreValues)
            {
                return false;
            }

            return true;
        }


    }

    string sample = """
                    ???.### 1,1,3
                    .??..??...?##. 1,1,3
                    ?#?#?#?#?#?#?#? 1,3,1,6
                    ????.#...#... 4,1,1
                    ????.######..#####. 1,6,5
                    ?###???????? 3,2,1
                    """;
}