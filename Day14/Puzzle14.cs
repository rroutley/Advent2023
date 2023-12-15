using System.Data;
using System.Runtime.CompilerServices;
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

        rocks = ParseLines(lines);
        Part2(rocks);

    }

    private void Part2(char[,] rocks)
    {
        // Run until reach steady state;
        var scores = new List<(long, long)>();
        var cycleScores = new List<(long, long)>();
        long hashcode = 0;

        int leadinLength = SpinUntilRepeat(rocks, scores, ref hashcode);

        int cycleLength = SpinUntilRepeat(rocks, cycleScores, ref hashcode);

        System.Console.WriteLine(leadinLength);
        System.Console.WriteLine(cycleLength);

        var answer = (1_000_000_000 - leadinLength - 1) % cycleLength;

        System.Console.WriteLine($"Answer = {cycleScores[answer].Item1}");



        int SpinUntilRepeat(char[,] rocks, List<(long, long)> scores, ref long hashcode)
        {
            var h = new HashSet<long>
            {
                hashcode
            };

            bool match;
            int cycleLength = 0;
            do
            {
                Spin(rocks, 1);
                hashcode = HashCode(rocks);
                scores.Add((Score(rocks), hashcode));

                match = h.Add(hashcode);
                cycleLength++;
            } while (match);
            return cycleLength;
        }
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

    private void Spin(char[,] rocks, long times)
    {
        for (long i = 0; i < times; i++)
        {
            TiltNorth(rocks);
            TiltWest(rocks);
            TiltSouth(rocks);
            TiltEast(rocks);
        }
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

    private void TiltWest(char[,] rocks)
    {
        bool changed;
        do
        {
            changed = false;
            for (var r = 0; r < rows; r++)
            {
                for (int c = cols - 2; c >= 0; c--)
                {
                    if (rocks[c, r] == '.' && rocks[c + 1, r] == 'O')
                    {
                        rocks[c, r] = 'O';
                        rocks[c + 1, r] = '.';
                        changed = true;
                    }

                }
            }
        } while (changed);
    }
    private void TiltSouth(char[,] rocks)
    {
        bool changed;
        do
        {
            changed = false;
            for (var r = 1; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (rocks[c, r] == '.' && rocks[c, r - 1] == 'O')
                    {
                        rocks[c, r] = 'O';
                        rocks[c, r - 1] = '.';
                        changed = true;
                    }

                }
            }
        } while (changed);
    }
    private void TiltEast(char[,] rocks)
    {
        bool changed;
        do
        {
            changed = false;
            for (var r = 0; r < rows; r++)
            {
                for (int c = 1; c < cols; c++)
                {
                    if (rocks[c, r] == '.' && rocks[c - 1, r] == 'O')
                    {
                        rocks[c, r] = 'O';
                        rocks[c - 1, r] = '.';
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

    long HashCode(char[,] rocks)
    {
        long score = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (rocks[c, r] == 'O')
                {
                    score += (rows - r) + (rows + 1) * c;
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