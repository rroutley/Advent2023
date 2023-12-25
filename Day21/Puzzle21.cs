using Point2d = (int x, int y);

class Puzzle21 : IPuzzle
{

    const byte Rock = 0;
    const byte Garden = 1;

    const byte Step = 2;

    private int rows;
    private int cols;
    public void Excute()
    {

        var lines = File.ReadAllLines("Day21/input.txt");
       // lines = sample.Replace("\n", "").Split(['\r']);

        rows = lines.Length;
        cols = lines[0].Length;

        var (start, grid) = ParseLines(lines);

        System.Console.WriteLine(start);

        //Part1(start, grid);

        Part2(start, grid, 1000);

        long s = 26501365;
        System.Console.WriteLine($"Answer = {(s + 1) * (s + 1)}");
    }

    private void Part1(Point2d start, byte[,] grid)
    {
        List<Point2d> spots = [start];

        for (int s = 0; s < 64; s++)
        {
            HashSet<Point2d> nextSpots = [];
            foreach (var spot in spots)
            {
                for (int d = 0; d < 4; d++)
                {
                    var direction = (Direction)d;
                    var next = Directions.From(direction, spot);
                    if (!Directions.IsWithinBounds(next, rows, cols))
                    {
                        continue;
                    }
                    if (grid[next.x, next.y] == Rock)
                    {
                        continue;
                    }
                    if (!nextSpots.Add(next))
                    {
                        continue;
                    }

                    nextSpots.Add(next);
                }
            }

            spots = [.. nextSpots];

            //            Dump(grid, spots, System.Console.Out);
        }

        var total = spots.Count();

        System.Console.WriteLine($"Answer ={total}");
    }


    private void Part2(Point2d start, byte[,] grid, int stepCount)
    {
        List<Point2d> spots = [start];
        using var writer = File.CreateText("day21/output.csv");
        for (int s = 0; s < stepCount; s++)
        {
            HashSet<Point2d> nextSpots = [];
            foreach (var spot in spots)
            {
                for (int d = 0; d < 4; d++)
                {
                    var direction = (Direction)d;
                    var next = Directions.From(direction, spot);
                    if (nextSpots.Contains(next))
                    {
                        continue;
                    }

                    var (x, y) = next;
                    x %= cols;
                    if (x < 0) x += cols;
                    y %= rows;
                    if (y < 0) y += rows;

                    if (grid[x, y] == Rock)
                    {
                        continue;
                    }


                    nextSpots.Add(next);
                }
            }

            spots = [.. nextSpots];

     //       Dump(grid, spots, System.Console.Out);


            writer.WriteLine($"{s + 1},{spots.Count}");
        }

        var total = spots.Count();

        System.Console.WriteLine($"Answer ={total}");
    }


    private void Dump(byte[,] grid, IEnumerable<Point2d> path, TextWriter writer)
    {
        for (int r = 0; r < rows; r++)
        {
            writer.Write($"{r:000}: ");
            for (int c = 0; c < cols; c++)
            {
                if (path.Contains((c, r)))
                {
                    System.Console.Write('O');
                }
                else
                {
                    char value = grid[c, r] switch
                    {
                        Rock => '#',
                        Garden => '.'
                    };
                    writer.Write(value);
                }
            }
            writer.WriteLine();
        }
        writer.WriteLine();
    }

    private (Point2d, byte[,]) ParseLines(string[] lines)
    {
        Point2d start = (0, 0);
        var grid = new byte[cols, rows];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var a = lines[r][c];

                if (a == 'S')
                {
                    start = (c, r);
                    grid[c, r] = Garden;
                }
                else if (a == '#')
                {
                    grid[c, r] = Rock;
                }
                else if (a == '.')
                {
                    grid[c, r] = Garden;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        return (start, grid);
    }

    string sample = """
                    ...........
                    .....###.#.
                    .###.##..#.
                    ..#.#...#..
                    ....#.#....
                    .##..S####.
                    .##..#...#.
                    .......##..
                    .##.#.####.
                    .##..##.##.
                    ...........
                    """;
}