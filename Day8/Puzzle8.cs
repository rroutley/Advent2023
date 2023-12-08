
using System.Data.Common;
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
        //Part1(instructions, directions);
        Part2(instructions, directions);
    }

    private static void Part1(char[] instructions, Dictionary<string, (string Left, string Right)> directions)
    {
        int p = 0;
        var pos = "AAA";
        while (pos != "ZZZ")
        {

            var direction = instructions[p % instructions.Length];
            p++;
            System.Console.WriteLine($"From {pos} go {direction}");
            pos = direction switch
            {
                'L' => directions[pos].Left,
                'R' => directions[pos].Right,
                _ => throw new NotImplementedException()
            };

        }
        System.Console.WriteLine($"Reached {pos} in {p} moves");
    }

    private static void Part2(char[] instructions, Dictionary<string, (string Left, string Right)> directions)
    {
        long p = 0;

        var positions = directions.Keys.Where(k => k.EndsWith('A')).ToList();

        System.Console.WriteLine(positions.Count);
        System.Console.WriteLine(instructions.Count());

        var cycles = new List<long>();

        for (int i = 0; i < positions.Count; i++)
        {
            var cycle = Part2a(instructions, directions, positions[i]);
            cycles.Add(cycle);
            System.Console.WriteLine($"{positions[i]} cycle len={cycle}");
        }

        System.Console.WriteLine("GCD = {0}", cycles.Aggregate(GCD));
        System.Console.WriteLine("Answer = LCM = {0}", cycles.Aggregate((x, y) => x * y / GCD(x, y)));

    }

    private static long Part2a(char[] instructions, Dictionary<string, (string Left, string Right)> directions, string start)
    {
        long p = 0;
        var pos = start;
        while (!pos.EndsWith('Z'))
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
    private static long GCD(long a, long b)
    {
        while (a != 0 && b != 0)
        {
            if (a > b)
                a %= b;
            else
                b %= a;
        }

        return a | b;
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