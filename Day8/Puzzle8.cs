
using System.Text.RegularExpressions;

class Puzzle8 : IPuzzle
{


    public void Excute()
    {

        var lines = File.ReadAllLines("Day8/input.txt");
        lines = sample.Replace("\n", "").Split(['\r']);

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

        bool done = false;
        while (!done)
        {

            var direction = instructions[p % instructions.Length];
            p++;

            done = true;
            for (int i = 0; i < positions.Count; i++)
            {
                var pos = positions[i];
                //      System.Console.WriteLine($"From {pos} go {direction}");
                pos = direction switch
                {
                    'L' => directions[pos].Left,
                    'R' => directions[pos].Right,
                    _ => throw new NotImplementedException()
                };

                if (!pos.EndsWith('Z'))
                    done = false;

                positions[i] = pos;
            }


            if (p % 100_000_000 == 0)
            {
                System.Console.WriteLine(p);
            }
        }
        System.Console.WriteLine($"Reached {string.Join(", ", positions)} in {p} moves");
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