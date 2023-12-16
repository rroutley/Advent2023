using System.Runtime.InteropServices;
using Point2D = (int x, int y);


class Puzzle16 : IPuzzle
{
    public void Excute()
    {

        var lines = File.ReadAllLines("Day16/input.txt");
        //  lines = sample.Replace("\n", "").Split(['\r']);

        Cell[,] grid = ParseLines(lines);

        var start = Cell.StartingAt((-1, 0), Direction.East);

        FollowBeams(grid, start);


        //Dump(grid, System.Console.Out);
        // using StreamWriter writer = File.CreateText("day16/output.txt");
        //Dump(grid, writer, true);

        var total = Score(grid);

        System.Console.WriteLine($"Answer = {total}");

        var bestStart = Part2(grid);
        FollowBeams(grid, bestStart);
        using StreamWriter writer = File.CreateText("day16/output.txt");
        Dump(grid, writer, true);


    }

    private Cell Part2(Cell[,] grid)
    {
        Cell bestStart = null;
        int maxScore = int.MinValue;
        
        for (int c = 0; c < cols; c++)
        {
            var start = Cell.StartingAt((c, -1), Direction.South);
            Check(start);

            start = Cell.StartingAt((c, rows), Direction.North);
            Check(start);
        }
        for (int r = 0; r < rows; r++)
        {
            var start = Cell.StartingAt((-1, r), Direction.East);
            Check(start);

            start = Cell.StartingAt((cols, r), Direction.West);
            Check(start);
        }
        System.Console.WriteLine($"Answer = {maxScore}");
        return bestStart;


        void Check(Cell start)
        {
            FollowBeams(grid, start);
            var score = Score(grid);
            if (score > maxScore)
            {
                maxScore = score;
                bestStart = start;
                System.Console.WriteLine(start);
            }
        }
    }

    private void FollowBeams(Cell[,] grid, Cell start)
    {
        Reset(grid);
        var history = new HashSet<(Direction, Point2D)>();

        Queue<Cell> queue = [];

        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            current.IsEnergized = true;
            foreach (var dir in current.BeamDirections)
            {
                if (!history.Add((dir, current.Position)))
                {
                    // A beam going in this direction has been here before.
                    continue;
                }


                var (dx, dy) = Directions.Deltas[dir];
                var x = current.Position.x + dx;
                var y = current.Position.y + dy;

                if (x < 0 || x >= cols || y < 0 || y >= rows)
                {
                    // Off the Grid, Beam is done
                    continue;
                }

                var next = grid[x, y];
                switch (next.Type)
                {
                    case '.':
                        next.BeamDirections.Add(dir);
                        break;
                    case '/':
                        {
                            var newDir = dir switch
                            {
                                Direction.North => Direction.East,
                                Direction.East => Direction.North,
                                Direction.South => Direction.West,
                                Direction.West => Direction.South,
                                _ => throw new NotImplementedException(),
                            };
                            next.BeamDirections.Add(newDir);
                            break;
                        }
                    case '\\':
                        {
                            var newDir = dir switch
                            {
                                Direction.North => Direction.West,
                                Direction.East => Direction.South,
                                Direction.South => Direction.East,
                                Direction.West => Direction.North,
                                _ => throw new NotImplementedException(),
                            };
                            next.BeamDirections.Add(newDir);
                            break;
                        }
                    case '|':
                        switch (dir)
                        {
                            case Direction.North:
                            case Direction.South:
                                next.BeamDirections.Add(dir);
                                break;
                            case Direction.East:
                            case Direction.West:
                                next.BeamDirections.Add(Direction.North);
                                next.BeamDirections.Add(Direction.South);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        break;
                    case '-':
                        switch (dir)
                        {
                            case Direction.East:
                            case Direction.West:
                                next.BeamDirections.Add(dir);
                                break;
                            case Direction.North:
                            case Direction.South:
                                next.BeamDirections.Add(Direction.East);
                                next.BeamDirections.Add(Direction.West);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
                queue.Enqueue(next);

            }

        }
    }

    private Cell[,] ParseLines(string[] lines)
    {
        rows = lines.Length;
        cols = lines[0].Length;

        var grid = new Cell[cols, rows];
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                grid[c, r] = new Cell((c, r), lines[r][c]);
            }
        }

        return grid;
    }

    private int Score(Cell[,] grid)
    {
        int score = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                score += grid[c, r].IsEnergized ? 1 : 0;
            }

        }
        return score;
    }

    private void Reset(Cell[,] grid)
    {
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                grid[c, r].Reset();
            }

        }
    }

    private void Dump(Cell[,] grid, TextWriter writer, bool mode = false)
    {
        for (int r = 0; r < rows; r++)
        {
            writer.Write($"{r:000}: ");
            for (int c = 0; c < cols; c++)
            {
                if (mode)
                {
                    Cell cell = grid[c, r];
                    if (cell.Type == '.')
                    {
                        if (cell.BeamDirections.Count == 1)
                        {
                            var dir = cell.BeamDirections.Single();
                            var sympol = dir switch
                            {
                                Direction.North => '^',
                                Direction.East => '>',
                                Direction.South => 'v',
                                Direction.West => '<',
                                _ => throw new NotImplementedException(),
                            };
                            writer.Write(sympol);
                        }
                        else if (cell.BeamDirections.Count > 1)
                        {
                            writer.Write(cell.BeamDirections.Count);
                        }
                        else
                        {
                            writer.Write(' ');
                        }

                    }
                    else
                    {
                        writer.Write(cell.Type);
                    }
                }
                else
                {
                    writer.Write(grid[c, r].IsEnergized ? '#' : '.');
                }
            }
            writer.WriteLine();
        }
        writer.WriteLine();
    }

    record Cell(Point2D Position, char Type)
    {
        public bool IsEnergized { get; set; }

        public HashSet<Direction> BeamDirections { get; private set; } = [];

        public void Reset()
        {
            IsEnergized = false;
            BeamDirections = [];
        }

        public static Cell StartingAt(Point2D position, Direction direction)
        {
            var cell = new Cell(position, '.');
            cell.BeamDirections.Add(direction);
            return cell;
        }
    }


    string sample = """
                    .|...\....
                    |.-.\.....
                    .....|-...
                    ........|.
                    ..........
                    .........\
                    ..../.\\..
                    .-.-/..|..
                    .|....-|.\
                    ..//.|....
                    """;
    private int rows;
    private int cols;
}
