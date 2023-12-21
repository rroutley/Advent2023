using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Dataflow;
using System.Xml;
using Point2D = (int x, int y);

class Puzzle18 : IPuzzle
{

    const int BLACK = 0;
    const int DARKGREY = 1;
    const int LIGHTGREY = 2;
    const int WHITE = 3;

    public void Excute()
    {

        var lines = File.ReadAllLines("Day18/input.txt");
        //lines = sample.Replace("\n", "").Split(['\r']);


        List<DigPlan> plan = [];
        foreach (var line in lines)
        {
            var match = Regex.Match(line, @"(\w) (\d+) \(#(.*)\)");

            var direction = match.Groups[1].Value switch
            {
                "R" => Direction.East,
                "U" => Direction.North,
                "D" => Direction.South,
                "L" => Direction.West,
                _ => throw new NotImplementedException(),
            };

            var amount = int.Parse(match.Groups[2].Value);
            var color = match.Groups[3].Value;


            // Part 2

            // amount = Convert.ToInt32(color.Substring(0, 5), 16);
            // direction = (Direction)((1 + int.Parse(color.Substring(5, 1))) % 4);

            plan.Add(new DigPlan(direction, amount, color));
        }

        var sf = plan.Where(s => s.Direction == Direction.East || s.Direction == Direction.West).Select(p => p.Amount).Aggregate(Numerics.Gcd);

        System.Console.WriteLine(sf);
        foreach (var item in plan)
        {
            System.Console.WriteLine(item);
        }


        var current = (0, 0);
        var path = new HashSet<Point2D>();

        foreach (var item in plan)
        {
            for (int i = 0; i < item.Amount; i++)
            {
                path.Add(current);
                current = item.Direction.From(current);
            }
        }



        Point2D topLeft = (path.Select(p => p.x).Min() - 1, path.Select(p => p.y).Min() - 1);
        Point2D bottomRight = (path.Select(p => p.x).Max() + 1, path.Select(p => p.y).Max() + 1);

        var grid = new int[bottomRight.x - topLeft.x + 1, bottomRight.y - topLeft.y + 1];
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                grid[x, y] = BLACK;
            }
        }

        foreach (var item in path)
        {
            var (x, y) = (item.x - topLeft.x, item.y - topLeft.y);
            grid[x, y] = DARKGREY;
        }

        FloodFill(grid, (0, 0), WHITE);

        //        var writer = System.Console.Out;

        var bmp = new PortableBitmap<int>(grid, WHITE, "Day 18 AOC");
        bmp.SaveAsPgm("day18/output.pgm");

        int count = 0;
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                count += grid[x, y] != WHITE ? 1 : 0;
            }
        }


        System.Console.WriteLine($"Answer ={count}");
    }

    private void FloodFill(int[,] canvas, Point2D value, int color)
    {
        var width = canvas.GetLength(0);
        var height = canvas.GetLength(1);

        var colorAtOrigin = canvas[value.x, value.y];
        var queue = new Queue<Point2D>();
        queue.Enqueue(value);
        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                var dir = (Direction)i;
                Point2D newPos = (x + Directions.Deltas[dir].x, y + Directions.Deltas[dir].y);
                // Check still within grid
                if (newPos.x < 0 || newPos.x >= width || newPos.y < 0 || newPos.y >= height)
                {
                    continue;
                }

                var pixel = canvas[newPos.x, newPos.y];
                if (pixel == colorAtOrigin)
                {
                    canvas[newPos.x, newPos.y] = color;
                    queue.Enqueue(newPos);
                }

            }

        }
    }
    record DigPlan(Direction Direction, int Amount, string Colour);

    string sample = """
                    R 6 (#70c710)
                    D 5 (#0dc571)
                    L 2 (#5713f0)
                    D 2 (#d2c081)
                    R 2 (#59c680)
                    D 2 (#411b91)
                    L 5 (#8ceee2)
                    U 2 (#caa173)
                    L 1 (#1b58a2)
                    U 2 (#caa171)
                    R 2 (#7807d2)
                    U 3 (#a77fa3)
                    L 2 (#015232)
                    U 2 (#7a21e3)
                    """;
}