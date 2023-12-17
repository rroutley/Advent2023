using Point2D = (int x, int y);

class Puzzle17 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day17/input.txt");
        lines = sample.Replace("\n", "").Split(['\r']);

        var grid = ParseLines(lines);


        var result = Astar(new Point2D(0, 0), new Point2D(cols - 1, rows - 01), Manhatten);

        Dump(grid, result, System.Console.Out);

        var total = rows;

        System.Console.WriteLine($"Answer ={total}");
    }

    // A* finds a path from start to goal.
    // h is the heuristic function. h(n) estimates the cost to reach goal from node n.

    private IEnumerable<Point2D> Astar(Point2D start, Point2D end, Func<Point2D, Point2D, int> h)
    {
        // The set of discovered nodes that may need to be (re-)expanded.
        // Initially, only the start node is known.
        // This is usually implemented as a min-heap or priority queue rather than a hash-set.
        PriorityQueue<Point2D, int> openSet = new();
        openSet.Enqueue(start, h(start, end));

        // For node n, cameFrom[n] is the node immediately preceding it on the cheapest path from the start
        // to n currently known.
        Dictionary<Point2D, Point2D> cameFrom = [];

        // For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
        Dictionary<Point2D, int> gScore = new()
        {
            [start] = 0,
        };

        // For node n, fScore[n] := gScore[n] + h(n). fScore[n] represents our current best guess as to
        // how cheap a path could be from start to finish if it goes through n.
        Dictionary<Point2D, int> fScore = new()
        {
            [start] = h(start, end),
        };

        while (openSet.Count > 0)
        {
            // This operation can occur in O(Log(N)) time if openSet is a min-heap or a priority queue
            var current = openSet.Dequeue();

            if (current == end)
            {
                return ReconstructPath(cameFrom, current);//?????
            }

            var neighbors = Directions.Deltas.Keys.Select(d => d.From(current));
            foreach (var neighbor in neighbors)
            {
                // d(current,neighbor) is the weight of the edge from current to neighbor
                // tentative_gScore is the distance from start to the neighbor through current
                var tentative_gScore = gScore[current] + d(current, neighbor);
                var g = gScore.ContainsKey(neighbor) ? gScore[neighbor] : int.MaxValue;
                if (tentative_gScore < g)
                {
                    // This path to neighbor is better than any previous one. Record it!
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentative_gScore;
                    fScore[neighbor] = tentative_gScore + h(neighbor, end);
                    if (!openSet.UnorderedItems.Any(i => i.Element == neighbor))
                    {
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }

            }

        }
        return null;
    }

    private int d((int x, int y) current, (int x, int y) neighbor)
    {
        return 1;
    }

    private IEnumerable<Point2D> ReconstructPath(Dictionary<Point2D, Point2D> cameFrom, Point2D current)
    {
        List<Point2D> totalPath = [current];
        while (cameFrom.Keys.Contains(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }

    static int Manhatten(Point2D a, Point2D b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }


    record State(Point2D position);



    private int[,] ParseLines(string[] lines)
    {
        rows = lines.Length;
        cols = lines[0].Length;

        var grid = new int[cols, rows];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                grid[c, r] = lines[r][c] - 48;
            }
        }

        return grid;
    }

    private void Dump(int[,] grid, IEnumerable<Point2D> path, TextWriter writer)
    {
        for (int r = 0; r < rows; r++)
        {
            writer.Write($"{r:000}: ");
            for (int c = 0; c < cols; c++)
            {


                if (path.Contains((c, r)))
                {
                    System.Console.Write('.');
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
                    2413432311323
                    3215453535623
                    3255245654254
                    3446585845452
                    4546657867536
                    1438598798454
                    4457876987766
                    3637877979653
                    4654967986887
                    4564679986453
                    1224686865563
                    2546548887735
                    4322674655533
                    """;
    private int rows;
    private int cols;
}