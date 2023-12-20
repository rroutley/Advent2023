using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Point2D = (int x, int y);

class Puzzle17 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day17/input.txt");
        lines = sample.Replace("\n", "").Split(['\r']);

        grid = ParseLines(lines);

        {
            var result = Astar(new Point2D(0, 0), new Point2D(cols - 1, rows - 1), Manhatten);

            Dump(grid, result, System.Console.Out);

            var total = Score(grid, result);

            System.Console.WriteLine($"Answer ={total}");
        }
        {
            var result = Disjktra(new Point2D(0, 0), new Point2D(cols - 1, rows - 1));

            Dump(grid, result, System.Console.Out);

            var total = Score(grid, result);

            System.Console.WriteLine($"Answer ={total}");
        }
    }


    public class Crucible
    {
        public Crucible(Point2D point, Crucible prev, int priority, Direction direction, int stepsInSameDirection)
        {
            Point = point;
            Priority = priority;
            Direction = direction;
            StepsInSameDirection = stepsInSameDirection;
            Previous = prev;
        }

        public Point2D Point { get; set; }

        public Crucible Previous { get; set; }

        public int Priority { get; set; }
        public Direction Direction { get; set; }
        public int StepsInSameDirection { get; set; }

        public static readonly IComparer<Crucible> PriorityOrder = new PriorityComparer();

        private class PriorityComparer : IComparer<Crucible>
        {
            public int Compare(Crucible x, Crucible y)
            {
                return x.Priority.CompareTo(y.Priority);
            }
        }
    }



    private IEnumerable<Point2D> Disjktra(Point2D start, Point2D end)
    {

        int minDistance = int.MaxValue;
        Crucible minItem = null;

        List<Crucible> queue =
        [
            new Crucible((1, 0), null, grid[1, 0], Direction.East, 1),
            new Crucible((0, 1), null, grid[0, 1], Direction.South, 1),
        ];

        HashSet<(Point2D, Direction)> seen = [];

        while (queue.Count > 0)
        {
            queue.Sort(Crucible.PriorityOrder);

            var item = queue[0];
            queue.RemoveAt(0);
            var current = item.Point;


            if (current == end)
            {
                if (item.Priority < minDistance)
                {
                    minItem = item;
                    minDistance = item.Priority;
                }
                continue;
            }

            if (!seen.Add((current, item.Direction)))
            {
                continue;
            }

            var neighbors = Directions.Deltas.Keys
                                                  .Where(d => !item.Direction.IsBackwards(d))
                                                  .Where(d => item.Direction != d || (item.Direction == d && item.StepsInSameDirection < 3))
                                                  .Select(d => (d, d.From(current)))
                                                  .Where(p => p.Item2.IsWithinBounds(rows, cols))
                                                  .ToList();

            foreach (var (dir, neighbor) in neighbors)
            {

                queue.Add(new Crucible(neighbor,
                                       item,
                                       item.Priority + d(current, neighbor, true),
                                       dir,
                                       item.Direction == dir ? item.StepsInSameDirection + 1 : 1));




            }

        }
        return ReconstructPath(minItem, start);
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
        Dictionary<Point2D, (Direction, Point2D)> cameFrom = [];

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
                return ReconstructPath(cameFrom, current);
            }

            var history = StepsInSameDirection(cameFrom, current);///????

            var neighbors = Directions.Deltas.Keys.Where(d => !history.direction.IsBackwards(d))
                                                  .Where(d => history.direction != d || (history.direction == d && history.steps < 3))
                                                  .Select(d => (d, d.From(current)))
                                                  .Where(p => p.Item2.IsWithinBounds(rows, cols));

            foreach (var (dir, neighbor) in neighbors)
            {
                // d(current,neighbor) is the weight of the edge from current to neighbor
                // tentative_gScore is the distance from start to the neighbor through current
                var tentative_gScore = gScore[current] + d(current, neighbor, history.direction != dir || (history.direction == dir && history.steps < 3));
                var g = gScore.ContainsKey(neighbor) ? gScore[neighbor] : int.MaxValue;
                if (tentative_gScore < g)
                {
                    // This path to neighbor is better than any previous one. Record it!
                    cameFrom[neighbor] = (dir, current);
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

    private static (Direction direction, int steps) StepsInSameDirection(Dictionary<Point2D, (Direction direction, Point2D point)> cameFrom, Point2D current)
    {
        if (!cameFrom.ContainsKey(current))
        {
            return (Direction.East, 0);
        }

        Direction last;
        int steps = 1;
        last = cameFrom[current].direction;
        current = cameFrom[current].point;
        while (true)
        {
            if (!cameFrom.ContainsKey(current))
            {
                break;
            }

            if (cameFrom[current].direction != last)
            {
                break;
            }
            current = cameFrom[current].point;
            steps++;
        }

        return (last, steps);
    }

    private int d(Point2D current, Point2D neighbor, bool passesThreeStepRule)
    {
        if (passesThreeStepRule)
        {
            return grid[neighbor.x, neighbor.y];
        }
        else
        {
            return int.MaxValue / 2;
        }
    }

    private IEnumerable<Point2D> ReconstructPath(Dictionary<Point2D, (Direction, Point2D)> cameFrom, Point2D current)
    {
        List<Point2D> totalPath = [current];
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current].Item2;
            totalPath.Insert(0, current);
        }
        return totalPath;
    }


    private IEnumerable<Point2D> ReconstructPath(Crucible current, Point2D start)
    {
        List<Point2D> totalPath = [current.Point];
        while (current != null)
        {
            totalPath.Insert(0, current.Point);
            current = current.Previous;
        }
        return totalPath;
    }

    static int Manhatten(Point2D a, Point2D b)
    {
        var dx = Math.Abs(a.x - b.x);
        var dy = Math.Abs(a.y - b.y);

        if (dx / 3 > dy)
        {
            return dx + dy + ((dx / 3 - dy) * 2);
        }
        if (dy / 3 > dx)
        {
            return dx + dy + ((dy / 3 - dx) * 2);
        }

        return dx + dy;
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
                if (path.Contains((c, r)) && !(r == 0 && c == 0))
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

    private int Score(int[,] grid, IEnumerable<Point2D> path)
    {
        int score = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (path.Contains((c, r)) && !(r == 0 && c == 0))
                {
                    score += grid[c, r];
                }
            }
        }
        return score;
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
    private int[,] grid;
}