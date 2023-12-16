using Point2D = (int x, int y);



class Puzzle10 : IPuzzle
{

    const char Vertical = '|';
    const char Horizontal = '-';
    const char TopLeft = 'F';
    const char BottomRight = 'J';
    const char TopRight = '7';
    const char BottomLeft = 'L';
    const char Start = 'S';
    const char Ground = '.';



    private IReadOnlyDictionary<char, Direction[]> validConnections = new Dictionary<char, Direction[]>
    {
        [Vertical] = [Direction.North, Direction.South],
        [Horizontal] = [Direction.East, Direction.West],
        [TopLeft] = [Direction.East, Direction.South],
        [BottomRight] = [Direction.North, Direction.West],
        [TopRight] = [Direction.West, Direction.South],
        [BottomLeft] = [Direction.North, Direction.East],
        [Start] = [Direction.North, Direction.East, Direction.South, Direction.West],
        [Ground] = [],
    };

    private IReadOnlyDictionary<char, int[]> bitmaps = new Dictionary<char, int[]>
    {
        [Vertical] = [
                0b010,
                0b010,
                0b010
                ],
        [Horizontal] = [
                0b000,
                0b111,
                0b000
                ],
        [TopLeft] = [
                0b000,
                0b011,
                0b010
                ],
        [BottomRight] = [
                0b010,
                0b110,
                0b000
                ],
        [TopRight] = [
                0b000,
                0b110,
                0b010
                ],
        [BottomLeft] = [
                0b010,
                0b011,
                0b000
                ],
        [Start] = [
                0b111,
                0b111,
                0b111
                ],
    };


    private int rows, cols;
    private char[,] grid;

    public void Excute()
    {

        var lines = File.ReadAllLines("Day10/input.txt");
        //lines = sample.Replace("\n", "").Split(['\r']);

        Point2D startingPosition = SetupGrid(lines);

        Part1(startingPosition);


        var points = FollowPath(startingPosition).ToList();

        DumpAsAscii(points);

        var canvas = MakeCanvas(points);
        FloodFill(canvas, (0, 0));

        SaveAsPgm(canvas, "Day10/output.pgm");

        var interoirCount = CountInterior(canvas);
        System.Console.WriteLine("Part 2 = {0}", interoirCount);
    }

    const byte Black = 0;
    const byte DarkGrey = 1;
    const byte LightGrey = 2;
    const byte White = 3;

    const byte NotSet = Black;
    const byte PipeColor = White;
    const byte PipeSurround = DarkGrey;
    const byte Outside = LightGrey;

    private int CountInterior(byte[,] canvas)
    {
        int interior = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                // Is the center pixel zero?
                if (canvas[3 * c + 1, 3 * r + 1] == Black)
                {
                    interior++;
                }
            }
        }
        return interior;
    }

    private void SaveAsPgm(byte[,] canvas, string fileName)
    {
        using var writer = File.CreateText(fileName);
        writer.NewLine = "\n";

        var height = canvas.GetLength(1);
        var width = canvas.GetLength(0);

        writer.WriteLine("P2");
        writer.WriteLine("# Day 10 AoC");
        writer.WriteLine($"{width} {height}");
        writer.WriteLine(White); // Maximum greyscale value
        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                writer.Write(canvas[c, r]);
                writer.Write(" ");
            }
            writer.WriteLine();
        }

    }


    private byte[,] MakeCanvas(List<Point2D> points)
    {
        var canvas = new byte[cols * 3, rows * 3];
        foreach (var (x, y) in points)
        {
            var bitmap = bitmaps[grid[x, y]];

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 3; i++)
                {
                    canvas[3 * x + i, 3 * y + j] = ((bitmap[j] & (4 >> i)) != 0) ? PipeColor : PipeSurround;
                }
            }
        }
        return canvas;
    }



    private void FloodFill(byte[,] canvas, Point2D value)
    {
        var width = canvas.GetLength(0);
        var height = canvas.GetLength(1);

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

                byte pixel = canvas[newPos.x, newPos.y];
                if (pixel == NotSet || pixel == PipeSurround)
                {
                    canvas[newPos.x, newPos.y] = Outside;
                    queue.Enqueue(newPos);
                }

            }

        }
    }

    private void DumpAsAscii(List<(int x, int y)> points)
    {
        var output = new char[rows][];
        for (int i = 0; i < rows; i++)
        {
            output[i] = new String(' ', cols).ToArray();
        }

        foreach (var (c, r) in points)
        {
            output[r][c] = grid[c, r];
        }

        for (int i = 0; i < rows; i++)
        {
            System.Console.WriteLine(new string(output[i]));
        }
    }

    private Point2D SetupGrid(string[] lines)
    {
        rows = lines.Length;
        cols = lines[0].Length;

        grid = new char[cols, rows];

        Point2D startingPosition = (-1, -1);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                char type = lines[r][c];
                grid[c, r] = type;

                var point = new Point2D(c, r);
                if (type == Start)
                {
                    startingPosition = point;
                }
            }
        }

        return startingPosition;
    }

    private void Part1(Point2D startingPosition)
    {
        int total = FollowPath(startingPosition).Count();

        System.Console.WriteLine("Answer = {0}", total / 2);
    }

    private IEnumerable<Point2D> FollowPath(Point2D startingPosition)
    {
        HashSet<Point2D> history = new HashSet<Point2D>();

        // Folow the pipes until we get back to start
        var total = 0;
        var position = startingPosition;
        System.Console.WriteLine($"Starting from {position}");
        do
        {
            var possible = CandidateMoves(position, skipChecks: total > 0);

            // No back tracking
            position = possible.Where(p => !history.Contains(p)).First();
            history.Add(position);

            yield return position;

        } while (position != startingPosition);

    }

    private IEnumerable<Point2D> CandidateMoves(Point2D position, bool skipChecks)
    {
        var validDirections = validConnections[grid[position.x, position.y]];
        foreach (var dir in validDirections)
        {
            Point2D newPos = (position.x + Directions.Deltas[dir].x, position.y + Directions.Deltas[dir].y);

            // Checks only needed for the Starting position
            if (!skipChecks)
            {
                // Check still within grid
                if (newPos.x < 0 || newPos.x >= cols || newPos.y < 0 || newPos.y >= rows)
                {
                    continue;
                }

                var oppositeDirection = (Direction)(((int)dir + 2) % 4);

                // Check can get back to initial position from here
                if (!validConnections[grid[newPos.x, newPos.y]].Contains(oppositeDirection))
                {
                    continue;
                }
            }
            yield return newPos;
        }
    }

    string sample = """
                    ..F7.
                    .FJ|.
                    SJ.L7
                    |F--J
                    LJ...
                    """;
}