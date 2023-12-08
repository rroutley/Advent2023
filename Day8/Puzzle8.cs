
using System.Security.Cryptography;
using System.Text.RegularExpressions;

class Puzzle8 : IPuzzle
{


    public void Excute()
    {

        var lines = File.ReadAllLines("Day8/input.txt");
        //   lines = sample.Replace("\n", "").Split(['\r']);

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



    string sample = """
                    LLR

                    AAA = (BBB, BBB)
                    BBB = (AAA, ZZZ)
                    ZZZ = (ZZZ, ZZZ)
                    """;
}