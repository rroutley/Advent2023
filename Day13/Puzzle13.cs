using System.Text;

class Puzzle13 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day13/input.txt");
        //    lines = sample.Replace("\n", "").Split(['\r']);

        var scenes = LoadScenes(lines);

        var total = 0;
        foreach (var scene in scenes)
        {
            var position = FindMirrorPosition(scene, 0);

            position = FlipUntilNewPosition(scene, position);

            System.Console.WriteLine(position);
            total += position;
        }

        System.Console.WriteLine($"Answer = {total}");
    }

    private int FlipUntilNewPosition(List<string> scene, int lastPosition)
    {
        for (int i = 0; i < scene[0].Length; i++)
        {
            for (int j = 0; j < scene.Count; j++)
            {
                // Flip one tile at j,i
                var temp = scene[j];
                var sb = new StringBuilder(scene[j]);
                sb[i] = temp[i] == '.' ? '#' : '.';
                scene[j] = sb.ToString();

                int position = FindMirrorPosition(scene, lastPosition);
                if (position > 0)
                {
                    return position;
                }

                // return to original 
                scene[j] = temp;

            }

        }
        throw new InvalidOperationException();
    }

    private int FindMirrorPosition(List<string> scene, int lastPosition)
    {
        if (TryFindHorizontal(scene, lastPosition / 100, out int position))
        {
            return position * 100;
        }

        else if (TryFindVertical(scene, lastPosition, out position))
        {
            return position;
        }
        return -1;
    }

    private bool TryFindVertical(List<string> scene, int lastPosition, out int position)
    {
        lastPosition--;
        for (int i = 0; i < scene[0].Length - 1; i++)
        {

            if (IsValid(i) && (i != lastPosition))
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

    private bool TryFindHorizontal(List<string> scene, int lastPosition, out int position)
    {
        lastPosition--;
        for (int i = 0; i < scene.Count - 1; i++)
        {
            if (IsValid(i) && (i != lastPosition))
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