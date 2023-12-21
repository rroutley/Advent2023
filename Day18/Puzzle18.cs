using System.Text.RegularExpressions;
using Point2D = (int x, int y);

class Puzzle18 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day18/input.txt");
        //   lines = sample.Replace("\n", "").Split(['\r']);

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

            amount = Convert.ToInt32(color[..5], 16);
            direction = (Direction)((1 + int.Parse(color.Substring(5, 1))) % 4);

            plan.Add(new DigPlan(direction, amount, color));
        }

        Point2D current = (0, 0);
        var path = new List<Point2D>();

        long perimeter = 0;
        foreach (var item in plan)
        {
            path.Add(current);

            var vector = Directions.Deltas[item.Direction].ToVector();
            current += vector * item.Amount;
            perimeter += item.Amount;
        }

        foreach (var p in path) System.Console.WriteLine(p);
        System.Console.WriteLine(current);


        // Use Trapezoid formula for area of a polygon
        long area = 0;
        for (int i = 0; i < path.Count; i++)
        {
            var j = (i + 1) % path.Count;
            var p0 = path[i];
            var p1 = path[j];
            long a = ((long)p0.y + p1.y) * ((long)p0.x - p1.x);
            area += a;
        }

        System.Console.WriteLine((area + perimeter) / 2 + 1);

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