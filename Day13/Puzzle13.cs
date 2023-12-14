

using System.Runtime.InteropServices.Marshalling;

class Puzzle13 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day13/input.txt");
        //lines = sample.Replace("\n", "").Split(['\r']);

        var scenes = LoadScenes(lines);

        var total = 0;
        foreach (var scene in scenes)
        {
            if (TryFindHorizontal(scene, out int position))
            {
                total += position * 100;
                System.Console.WriteLine(position);
            }

            if (TryFindVertical(scene, out position))
            {
                total += position;
                System.Console.WriteLine(position);
            }
        }

        System.Console.WriteLine($"Answer = {total}");
    }

    private bool TryFindVertical(List<string> scene, out int position)
    {
        for (int i = 0; i < scene[0].Length - 1; i++)
        {

            if (IsValid(i))
            {
                position = i + 1; // Answer is 1 based 
                return true;
            }

        }
        position = -1;
        return false;


        bool IsValid(int position)
        {
            int i = position, j = position + 1;
            while (i >= 0 && j < scene[0].Length)
            {
                for (int k = 0; k < scene.Count; k++)
                {
                    if (scene[k][i] != scene[k][j])
                    {
                        return false;
                    }
                }
                i--;
                j++;
            }
            return true;
        }
    }

    private bool TryFindHorizontal(List<string> scene, out int position)
    {
        for (int i = 0; i < scene.Count - 1; i++)
        {

            if (IsValid(i))
            {
                position = i + 1; // Answer is 1 based 
                return true;
            }

        }
        position = -1;
        return false;

        bool IsValid(int position)
        {
            int i = position, j = position + 1;
            while (i >= 0 && j < scene.Count)
            {
                if (scene[i] != scene[j])
                {
                    return false;
                }
                i--;
                j++;
            }
            return true;
        }

    }


    private static List<List<string>> LoadScenes(string[] lines)
    {
        var scenes = new List<List<string>>();

        var scene = new List<string>();
        foreach (var line in lines)
        {

            if (string.IsNullOrWhiteSpace(line))
            {
                scenes.Add(scene);
                scene = new List<string>();
            }
            else
            {
                scene.Add(line);
            }

        }

        scenes.Add(scene);

        return scenes;
    }

    string sample = """
                    #.##..##.
                    ..#.##.#.
                    ##......#
                    ##......#
                    ..#.##.#.
                    ..##..##.
                    #.#.##.#.

                    #...##..#
                    #....#..#
                    ..##..###
                    #####.##.
                    #####.##.
                    ..##..###
                    #....#..#
                    """;
}