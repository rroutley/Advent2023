

class Puzzle24 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day24/input.txt");
        //    lines = sample.Replace("\n", "").Split(['\r']);


        var hailstones = new List<Hailstone>();
        foreach (var line in lines)
        {
            var (pos, vel) = line.Split('@', 2);
            var hailstone = new Hailstone(Vector3d.Parse(pos), Vector3d.Parse(vel));
            hailstones.Add(hailstone);
        }

        Part1(hailstones);
    }

    private static void Part1(List<Hailstone> hailstones)
    {
        var testAreaMin = 200000000000000;
        var testAreaMax = 400000000000000;
        // testAreaMin = 7;
        // testAreaMax = 27;

        int total = 0;
        for (int i = 0; i < hailstones.Count; i++)
        {
            for (int j = i + 1; j < hailstones.Count; j++)
            {

                var h1 = hailstones[i];
                var h2 = hailstones[j];

                var d = h1.Velocity.X * h2.Velocity.Y - h1.Velocity.Y * h2.Velocity.X;
                double t = double.PositiveInfinity;
                double u = double.PositiveInfinity;

                if (d != 0)
                {
                    t = ((h1.Position.X - h2.Position.X) * -h2.Velocity.Y - (h1.Position.Y - h2.Position.Y) * -h2.Velocity.X) / d;

                    u = ((h1.Position.X - h2.Position.X) * -h1.Velocity.Y - (h1.Position.Y - h2.Position.Y) * -h1.Velocity.X) / d;
                }

                var p = h1.Position + h1.Velocity * t;

                var pass = t > 0 && u > 0 && (testAreaMin <= p.X && p.X <= testAreaMax && testAreaMin <= p.Y && p.Y <= testAreaMax);

                if (pass) total++;

                if (pass)
                    System.Console.WriteLine($"{h1.Position}, {h2.Position} => {p} {pass} {t} {u}");

            }
        }




        System.Console.WriteLine($"Answer ={total}");
    }

    record Hailstone(Vector3d Position, Vector3d Velocity);
    string sample = """
                    19, 13, 30 @ -2,  1, -2
                    18, 19, 22 @ -1, -1, -2
                    20, 25, 34 @ -2, -2, -4
                    12, 31, 28 @ -1, -2, -1
                    20, 19, 15 @  1, -5, -3
                    """;
}