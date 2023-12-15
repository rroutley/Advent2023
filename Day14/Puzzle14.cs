using System.Data;
using Point2D = (int x, int y);

class Puzzle14 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day14/input.txt");
        // lines = sample.Replace("\n", "").Split(['\r']);

        rows = lines.Length;
        cols = lines[0].Length;

        char[,] rocks = ParseLines(lines);
        Dump(rocks);

        TiltNorth(rocks);

        Dump(rocks);

        var total = Score(rocks);

        System.Console.WriteLine($"Answer ={total}");

    }

    private char[,] ParseLines(string[] lines)
    {
        var rocks = new char[cols, rows];
        var roundRocks = new List<Point2D>();
        var cubeRocks = new List<Point2D>();
        for (int r = 0; r < lines.Length; r++)
        {
            var line = lines[r];
            for (int c = 0; c < lines.Length; c++)
            {
                rocks[c, r] = line[c];

                if (line[c] == 'O')
                {
                    roundRocks.Add(new Point2D(c, r));
                }
                else if (line[c] == '#')
                {
                    cubeRocks.Add(new Point2D(c, r));
                }
            }
        }

        return rocks;
    }

    private void TiltNorth(char[,] rocks)
    {
        bool changed;
        do
        {
            changed = false;
            for (var r = rows - 2; r >= 0; r--)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (rocks[c, r] == '.' && rocks[c, r + 1] == 'O')
                    {
                        rocks[c, r] = 'O';
                        rocks[c, r + 1] = '.';
                        changed = true;
                    }

                }
            }
        } while (changed);
    }

    long Score(char[,] rocks)
    {
        long score = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (rocks[c, r] == 'O')
                {
                    score += rows - r;
                }
            }
        }
        return score;
    }



    void Dump(char[,] rocks)
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                System.Console.Write(rocks[c, r]);
            }
            System.Console.WriteLine();
        }
        System.Console.WriteLine();
    }

    string sample = """
                    O....#....
                    O.OO#....#
                    .....##...
                    OO.#O....O
                    .O.....O#.
                    O.#..O.#.#
                    ..O..#O..O
                    .......O..
                    #....###..
                    #OO..#....
                    """;
    private int rows;
    private int cols;
}