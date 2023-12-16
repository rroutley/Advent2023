using Point2D = (int x, int y);


class Puzzle16 : IPuzzle
{
    int beams = 0;

    public void Excute()
    {

        var lines = File.ReadAllLines("Day16/input.txt");
        lines = sample.Replace("\n", "").Split(['\r']);

        Cell[,] grid = ParseLines(lines);

        var start = grid[0, 0];
        start.BeamDirections.Add((beams++, Direction.East));

        FollowBeams(grid, start);


        Dump(grid);

        var total = Score(grid);

        System.Console.WriteLine($"Answer ={total}");
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

    private void Dump(Cell[,] grid)
    {
        for (int r = 0; r < rows; r++)
        {
            System.Console.Write($"{r:000}: ");
            for (int c = 0; c < cols; c++)
            {
                System.Console.Write(grid[c, r].IsEnergized ? '#' : '.');
            }
            System.Console.WriteLine();
        }
        System.Console.WriteLine();
    }

    private void FollowBeams(Cell[,] grid, Cell start)
    {

        var history = new HashSet<(Direction, Point2D)>();

        Queue<Cell> queue = [];

        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();


            current.IsEnergized = true;
            foreach (var (beam, dir) in current.BeamDirections)
            {
                if (!history.Add((dir, current.position)))
                {
                    // this beam has been here before.
                    continue;
                }


                var (dx, dy) = Directions.Deltas[dir];
                var x = current.position.x + dx;
                var y = current.position.y + dy;

                if (x < 0 || x >= cols || y < 0 || y >= rows)
                {
                    // Off the Grid, Beam is done
                    continue;
                }

                var next = grid[x, y];
                switch (next.type)
                {
                    case '.':
                        next.BeamDirections.Add((beam, dir));
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
                            next.BeamDirections.Add((beam, newDir));
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
                            next.BeamDirections.Add((beam, newDir));
                            break;
                        }
                    case '|':
                        switch (dir)
                        {
                            case Direction.North:
                            case Direction.South:
                                next.BeamDirections.Add((beam, dir));
                                break;
                            case Direction.East:
                            case Direction.West:
                                next.BeamDirections.Add((beams++, Direction.North));
                                next.BeamDirections.Add((beam, Direction.South));
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
                                next.BeamDirections.Add((beam, dir));
                                break;
                            case Direction.North:
                            case Direction.South:
                                next.BeamDirections.Add((beams++, Direction.East));
                                next.BeamDirections.Add((beam, Direction.West));
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

    record Cell(Point2D position, char type)
    {
        public bool IsEnergized { get; set; }

        public HashSet<(int, Direction)> BeamDirections { get; } = [];
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
