using Point2d = (int x, int y);

class Puzzle23 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day23/input.txt");
        //lines = sample.Replace("\n", "").Split(['\r']);

        rows = lines.Length;
        cols = lines[0].Length;


        (int x, int y) start, end;
        char[,] grid = ParseLines(lines, out start, out end);

        Dump(grid, [], System.Console.Out);
        System.Console.WriteLine($"{start} to {end}");


        Heap<(Point2d, SortedSet<Point2d>)> queue = new(Heap<Point2d>.MaxHeap);

        queue.Enqueue((start, []), 0);

        var bestPath = new SortedSet<Point2d>();

        while (queue.Count > 0)
        {
            var (spot, history) = queue.Dequeue();

            if (spot == end)
            {
                System.Console.WriteLine($"At End  {history.Count}");
                if (history.Count > bestPath.Count)
                {
                    bestPath = history;
                    continue;
                }
            }


            history.Add(spot);

            for (int d = 0; d < 4; d++)
            {
                var direction = (Direction)d;

                var next = direction.From(spot);
                if (!Directions.IsWithinBounds(next, rows, cols))
                {
                    continue;
                }

                if (grid[next.x, next.y] == '#')
                {
                    continue;
                }

                if (history.Contains(next))
                {
                    continue;
                }

                bool canMove = grid[next.x, next.y] switch
                {
                    '^' => direction == Direction.North,
                    '>' => direction == Direction.East,
                    'v' => direction == Direction.South,
                    '<' => direction == Direction.West,
                    '.' => true,
                    _ => throw new NotImplementedException(),
                };

                // if (!canMove)
                // {
                //     continue;
                // }

                int s = history.Count;

                queue.Enqueue((next, [.. history]), s);

            }
        }


        //var route = ReconstructPath(path, end);

        Dump(grid, bestPath, System.Console.Out);

        var total = bestPath.Count;

        System.Console.WriteLine($"Answer ={total}");
    }

    private char[,] ParseLines(string[] lines, out (int x, int y) start, out (int x, int y) end)
    {
        var grid = new char[cols, rows];
        start = (0, 0);
        end = (0, 0);
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (r == 0 && lines[r][c] == '.')
                {
                    start = (c, r);
                }
                if (r == rows - 1 && lines[r][c] == '.')
                {
                    end = (c, r);
                }

                grid[c, r] = lines[r][c];
            }
        }
        return grid;
    }

    private IEnumerable<Point2d> ReconstructPath(Dictionary<Point2d, Point2d> cameFrom, Point2d current)
    {
        List<Point2d> totalPath = [current];
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }



    private void Dump(char[,] grid, IEnumerable<Point2d> path, TextWriter writer)
    {
        for (int r = 0; r < rows; r++)
        {
            writer.Write($"{r:000}: ");
            for (int c = 0; c < cols; c++)
            {
                if (path.Contains((c, r)))
                {
                    writer.Write('O');
                }
                else
                {
                    writer.Write(grid[c, r]);
                }
            }
            writer.WriteLine();
        }
        writer.WriteLine();
    }

    string sample = """
                    #.#####################
                    #.......#########...###
                    #######.#########.#.###
                    ###.....#.>.>.###.#.###
                    ###v#####.#v#.###.#.###
                    ###.>...#.#.#.....#...#
                    ###v###.#.#.#########.#
                    ###...#.#.#.......#...#
                    #####.#.#.#######.#.###
                    #.....#.#.#.......#...#
                    #.#####.#.#.#########v#
                    #.#...#...#...###...>.#
                    #.#.#v#######v###.###v#
                    #...#.>.#...>.>.#.###.#
                    #####v#.#.###v#.#.###.#
                    #.....#...#...#.#.#...#
                    #.#########.###.#.#.###
                    #...###...#...#...#.###
                    ###.###.#.###v#####v###
                    #...#...#.#.>.>.#.>.###
                    #.###.###.#.###.#.#v###
                    #.....###...###...#...#
                    #####################.#
                    """;
    private int rows;
    private int cols;
}