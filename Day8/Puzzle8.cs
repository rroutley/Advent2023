using System.Text.RegularExpressions;

class Puzzle8 : IPuzzle
{
    public void Excute()
    {

        var lines = File.ReadAllLines("Day8/input.txt");
        //lines = sample.Replace("\n", "").Split(['\r']);

        var instructions = lines[0].ToArray();

        var directions = new Dictionary<string, (string Left, string Right)>();

        for (int i = 2; i < lines.Length; i++)
        {
            var line = lines[i];

            var match = Regex.Match(line, @"(\w+) = \((\w+), (\w+)\)");

            var start = match.Groups[1].Value;
            var left = match.Groups[2].Value;
            var right = match.Groups[3].Value;

            directions.Add(start, (left, right));

        }
        Part1(instructions, directions);

        Part2(instructions, directions);
    }

    private static void Part1(char[] instructions, Dictionary<string, (string Left, string Right)> directions)
    {
        Part1(instructions, directions, "AAA", pos => pos == "ZZZ");
    }

    private static long Part1(char[] instructions, Dictionary<string, (string Left, string Right)> directions, string start, Func<string, bool> predicate)
    {
        int p = 0;
        var pos = start;
        while (!predicate(pos))
        {

            var direction = instructions[p % instructions.Length];
            p++;
            //  System.Console.WriteLine($"From {pos} go {direction}");
            pos = direction switch
            {
                'L' => directions[pos].Left,
                'R' => directions[pos].Right,
                _ => throw new NotImplementedException()
            };

        }
        System.Console.WriteLine($"Reached {pos} in {p} moves");
        return p;
    }

    private static void Part2(char[] instructions, Dictionary<string, (string Left, string Right)> directions)
    {
        var startPositions = directions.Keys.Where(k => k.EndsWith('A')).ToArray();

        System.Console.WriteLine(startPositions.Length);
        System.Console.WriteLine(instructions.Length);

        var cycles = new List<long>();

        foreach (var position in startPositions)
        {
            var cycle = Part1(instructions, directions, position, pos => pos.EndsWith('Z'));
            cycles.Add(cycle);

            System.Console.WriteLine($"{position} cycle len={cycle}");
        }

        System.Console.WriteLine("GCD = {0}", cycles.Aggregate(Numerics.Gcd));
        System.Console.WriteLine("Answer = LCM = {0}", cycles.Aggregate(Numerics.Lcm));

    }

    string sample = """
                    LR

                    11A = (11B, XXX)
                    11B = (XXX, 11Z)
                    11Z = (11B, XXX)
                    22A = (22B, XXX)
                    22B = (22C, 22C)
                    22C = (22Z, 22Z)
                    22Z = (22B, 22B)
                    XXX = (XXX, XXX)
                    """;
}