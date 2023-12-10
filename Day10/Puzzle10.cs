using System.Data;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

using Point2D = (int x, int y);
using Thing = (int dx, int dy, Direction);

enum Direction
{
    North, East, South, West
}

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


    private readonly IReadOnlyDictionary<Direction, Point2D> deltas = new Dictionary<Direction, Point2D>
    {
        [Direction.North] = (0, -1),
        [Direction.East] = (1, 0),
        [Direction.South] = (0, 1),
        [Direction.West] = (-1, 0),
    };

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

    private int rows, cols;
    private char[,] grid;

    public void Excute()
    {

        var lines = File.ReadAllLines("Day10/input.txt");
        //lines = sample.Replace("\n", "").Split(['\r']);

        Point2D startingPosition = SetupGrid(lines);

        Part1(startingPosition);

    }

    private (int x, int y) SetupGrid(string[] lines)
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

    private void Part1((int x, int y) startingPosition)
    {
        HashSet<Point2D> history = new HashSet<Point2D>();

        // Folow the pipes until we get back to start
        var total = 0;
        var position = startingPosition;
        System.Console.WriteLine($"Starting from {position}");
        do
        {
            var possible = CandidateMoves(position, total > 0);

            // No back tracking
            position = possible.Where(p => !history.Contains(p)).First();
            history.Add(position);

            total++;

        } while (position != startingPosition);

        System.Console.WriteLine("Answer = {0}", total / 2);
    }

    private IEnumerable<Point2D> CandidateMoves(Point2D position, bool skipChecks)
    {
        var validDirections = validConnections[grid[position.x, position.y]];
        foreach (var dir in validDirections)
        {
            Point2D newPos = (position.x + deltas[dir].x, position.y + deltas[dir].y);

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