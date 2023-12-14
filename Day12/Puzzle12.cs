
using System.Text.RegularExpressions;

class Puzzle12 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day12/input.txt");
        //        lines = sample.Replace("\n", "").Split(['\r']);

        // var test = ParseLine(".??..??...?##. 1,1,3");
        // System.Console.WriteLine(test.IsValid("..#...#...###."));
        // System.Console.WriteLine(test.IsValid("..#..#....###."));
        // System.Console.WriteLine(test.IsValid(".#....#...###."));
        // System.Console.WriteLine(test.IsValid(".#...#....###."));

        // System.Console.WriteLine(test.IsValid(".##..##...###."));
        // System.Console.WriteLine(test.IsValid(".#.#.##...###."));

        // test = ParseLine(".??..??...?##. 1,1,3");
        // test = test.Unfold(2);

        // System.Console.WriteLine(test.IsValid(".#...#....###.?.??..??...?##.", 14));

        var total = 0;
        var completed = 0;
        object locker = new object();

        Parallel.ForEach(lines, line =>
        {
            ConditionMap condition = ParseLine(line);
            condition = condition.Unfold(5);

            // System.Console.WriteLine($"{condition} =>");
            var combinations = condition.FindCombinations();
            // foreach (var result in combinations)
            // {
            //     System.Console.WriteLine($"  {result}");
            //     total++;
            // }
            lock (locker)
            {
                total += combinations.Count();
                completed++;
            }
            System.Console.WriteLine($"{condition} =>  {combinations.Count()} : {completed} completed");


            System.Console.WriteLine();
        });

        System.Console.WriteLine($"Answer ={total}");
    }

    private static ConditionMap ParseLine(string line)
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
                // Check Validity up to the current starting point
                if (!IsValid(candidate, start))
                {
                    return;
                }
                else if (start == candidate.Length)
                {
                    // We have a valid combination of the correct length
                    results.Add(candidate);
                    return;
                }


                for (int i = start; i < candidate.Length; i++)
                {
                    var c = candidate[i];

                    if (c == '?')
                    {
                        // Try replacing the ? with a .
                        Recurse(candidate[..i] + '.' + candidate[(i + 1)..], i + 1);

                        // Try replacing the ? with a #
                        Recurse(candidate[..i] + '#' + candidate[(i + 1)..], i + 1);

                    }

                }


                Recurse(candidate, candidate.Length);


            }

        }

        public bool IsValid(string candidate, int end = int.MaxValue)
        {

            var values = Values.GetEnumerator();
            var hasMoreValues = values.MoveNext();
            int digitsRemaining = 0;
            bool inValue = false;

            if (candidate.Length != Map.Length)
            {
                return false;
            }

            for (int i = 0; i < candidate.Length && i < end; i++)
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

            // Check end condition if we are testing the whole string
            if (end >= candidate.Length)
            {
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
            }
            else
            {
                // will the remining values fit in the rest of the groups?
                // var fixedGroups = Regex.Matches(candidate[end..], @"[\?#]+").Select(m => m.Groups[0].Value.Length).ToArray();
                // if (inValue)
                // {
                //     hasMoreValues = values.MoveNext();
                //     if (fixedGroups.Length == 0)
                //     {
                //         return false;
                //     }
                //     fixedGroups = fixedGroups[1..];

                // }

                // int i = 0;
                // while (hasMoreValues)
                // {
                //     if (i >= fixedGroups.Length)
                //     {
                //         return false;
                //     }
                //     if (values.Current > fixedGroups[i++])
                //     {
                //         continue;
                //     }
                //     hasMoreValues = values.MoveNext();
                // }

            }

            return true;
        }


        public ConditionMap Unfold(int times)
        {
            var fiveFold = Enumerable.Range(0, times);

            var map = string.Join("?", fiveFold.Select(f => Map));
            var values = fiveFold.SelectMany(f => Values).ToList();

            return new ConditionMap(map, values);
        }



        public override string ToString()
        {
            return $"{Map} {string.Join(',', Values)}";
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