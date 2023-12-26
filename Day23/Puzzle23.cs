using Point2d = (int x, int y);

class Puzzle23 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day23/input.txt");
        //    lines = sample.Replace("\n", "").Split(['\r']);

        rows = lines.Length;
        cols = lines[0].Length;

        var grid = new char[cols, rows];
        Point2d start = (0, 0);
        Point2d end = (0, 0);
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


        Dump(grid, [], System.Console.Out);
        System.Console.WriteLine($"{start} to {end}");


        Dictionary<Point2d, int> distances = new Dictionary<Point2d, int>
        {
            [start] = 0
        };

        Heap<(Point2d, HashSet<Point2d>)> queue = new(Heap<Point2d>.MaxHeap);
        Dictionary<Point2d, Point2d> path = [];

        queue.Enqueue((start, []), 0);

        while (queue.Count > 0)
        {
            var (spot, history) = queue.Dequeue();

            if (spot == end)
            {
                System.Console.WriteLine($"At End  {distances[spot]}");
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

                if (!canMove)
                {
                    continue;
                }

                if (!distances.ContainsKey(next) || distances[next] < distances[spot] + 1)
                {
                    distances[next] = distances[spot] + 1;
                    queue.Enqueue((next, [.. history]), distances[next]);
                    path[next] = spot;
                }
            }
        }


        var route = ReconstructPath(path, end);

        Dump(grid, route, System.Console.Out);

        var total = distances[end];

        System.Console.WriteLine($"Answer ={total}");
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