using Point2D = (int x, int y);


class Puzzle11 : IPuzzle
{

    int rows;
    int cols;


    public void Excute()
    {

        var lines = File.ReadAllLines("Day11/input.txt");
        // lines = sample.Replace("\n", "").Split(['\r']);

        var galaxies = ParseLines(lines);

        var universe = ApplyExpansion(galaxies);

        Part1(universe);
    }

    private static void Part1(List<(int x, int y)> universe)
    {
        var total = 0;
        var history = new HashSet<(int, int)>();
        for (int i = 0; i < universe.Count; i++)
        {
            var g1 = universe[i];
            for (int j = 0; j < universe.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }

                if (history.Contains((i, j)) || history.Contains((j, i)))
                {
                    continue;
                }

                history.Add((i, j));

                var g2 = universe[j];

                var dist = Math.Abs(g1.x - g2.x) + Math.Abs(g1.y - g2.y);

                //      System.Console.WriteLine($"Dist from Galacy {i + 1} {g1} to Galacy {j + 1} {g2} is {dist} units");

                total += dist;
            }
        }
        System.Console.WriteLine($"Answer = {total}");
    }

    private List<Point2D> ApplyExpansion(List<MutablePoint> galaxies)
    {
        var allRows = Enumerable.Range(0, rows);
        var allCols = Enumerable.Range(0, cols);

        var missingCols = allCols.Where(c => !galaxies.Any(g => g.x == c)).OrderByDescending(c => c).ToList();
        var missingRows = allCols.Where(r => !galaxies.Any(g => g.y == r)).OrderByDescending(r => r).ToList();

        foreach (var col in missingCols)
        {
            InsertColumnAt(col);
            cols++;
        }

        foreach (var row in missingRows)
        {
            InsertRowAt(row);
            rows++;
        }

        return galaxies.Select(p => p.ToPoint()).ToList();

        void InsertColumnAt(int col)
        {
            foreach (var g in galaxies)
            {
                if (g.x >= col)
                {
                    g.x++;
                }
            }
        }
        void InsertRowAt(int col)
        {
            foreach (var g in galaxies)
            {
                if (g.y >= col)
                {
                    g.y++;
                }
            }
        }
    }



    private List<MutablePoint> ParseLines(string[] lines)
    {
        var galaxies = new List<MutablePoint>();
        rows = lines.Length;
        cols = lines[0].Length;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                char type = lines[r][c];

                if (type == '#')
                {
                    var galaxy = new MutablePoint(c, r);
                    galaxies.Add(galaxy);
                }
            }
        }

        return galaxies;
    }
    class MutablePoint
    {
        public int x;
        public int y;

        public MutablePoint(int c, int r)
        {
            x = c;
            y = r;
        }

        public Point2D ToPoint()
        {
            return (x, y);
        }
    }

    string sample = """
                    ...#......
                    .......#..
                    #.........
                    ..........
                    ......#...
                    .#........
                    .........#
                    ..........
                    .......#..
                    #...#.....
                    """;
}