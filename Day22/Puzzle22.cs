using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using Point3d = (int x, int y, int z);

class Puzzle22 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day22/input.txt");
        //lines = sample.Replace("\n", "").Split(['\r']);

        List<Brick> bricks = [];

        int b = 0;
        foreach (var line in lines)
        {
            var brick = Brick.Parse(line, ++b);
            bricks.Add(brick);
        }

        bricks.Sort(new BrickHeightComparer());

        var maxX = bricks.Max(b => Math.Max(b.Start.x, b.End.x)) + 1;
        var maxY = bricks.Max(b => Math.Max(b.Start.y, b.End.y)) + 1;
        var maxZ = bricks.Max(b => Math.Max(b.Start.z, b.End.z)) + 1;
        Point3d dim = (maxX, maxY, maxZ);

        System.Console.WriteLine(dim);

        bricks = DropAll(bricks, dim);
        maxZ = bricks.Max(b => Math.Max(b.Start.z, b.End.z)) + 2;
        dim = (maxX, maxY, maxZ);

        int[,,] space = Map(bricks, dim);

        Dump(dim, space);

        foreach (var brick in bricks)
        {
            var supports = Supports(space, dim, brick, bricks);
            brick.Supports = supports;

            var supportedBy = SupportedBy(space, dim, brick, bricks);
            brick.SupportedBy = supportedBy;

            if (!supportedBy.Any() && brick.Start.z != 1)
            {
                throw new NotImplementedException();
            }

            // System.Console.WriteLine($"{brick} Supports {string.Join(", ", supports.Select(s => s.ToString()))}");
            // System.Console.WriteLine($"{brick} Supported By {string.Join(", ", supportedBy.Select(s => s.ToString()))}");
        }

        var total = 0;
        foreach (var brick in bricks)
        {
            var canDisintegrate = CanDisintegrate(brick);

            if (canDisintegrate) total++;
            //            System.Console.WriteLine($"{brick} Can Be Disintgrated {canDisintegrate}");
        }

        System.Console.WriteLine($"Answer ={total}");
    }

    private bool CanDisintegrate(Brick brick)
    {
        var supporting = brick.Supports;

        if (!supporting.Any())
        {
            return true;
        }

        var otherSupports = from s in supporting
                            from k in s.SupportedBy
                            where k != brick
                            select k;

        if (otherSupports.Any())
        {
            return true;
        }

        return false;
    }
    private IEnumerable<Brick> Supports(int[,,] space, Point3d dim, Brick brick, List<Brick> bricks)
    {
        var maxZ = Math.Max(brick.Start.z, brick.End.z);

        HashSet<Brick> supports = [];
        for (int x = brick.Start.x; x <= brick.End.x; x++)
        {
            for (int y = brick.Start.y; y <= brick.End.y; y++)
            {
                int z = maxZ + 1;
                int number = space[x, y, z];
                if (number != 0)
                {
                    Brick item = bricks.Single(b => b.Number == number);
                    if (item.Number != number) throw new InvalidOperationException();

                    supports.Add(item);
                };
            }
        }
        return supports;
    }
    private IEnumerable<Brick> SupportedBy(int[,,] space, Point3d dim, Brick brick, List<Brick> bricks)
    {
        var minZ = Math.Min(brick.Start.z, brick.End.z);

        HashSet<Brick> supportedBy = [];
        for (int x = brick.Start.x; x <= brick.End.x; x++)
        {
            for (int y = brick.Start.y; y <= brick.End.y; y++)
            {
                int z = minZ - 1;
                int number = space[x, y, z];
                if (number != 0)
                {
                    Brick item = bricks.Single(b => b.Number == number);
                    if (item.Number != number) throw new InvalidOperationException();

                    supportedBy.Add(item);
                };
            }
        }
        return supportedBy;
    }

    private static int[,,] Map(List<Brick> bricks, Point3d dim)
    {
        var space = new int[dim.x, dim.y, dim.z];

        foreach (Brick brick in bricks)
        {
            for (int x = brick.Start.x; x <= brick.End.x; x++)
            {
                for (int y = brick.Start.y; y <= brick.End.y; y++)
                {
                    for (int z = brick.Start.z; z <= brick.End.z; z++)
                    {
                        space[x, y, z] = brick.Number;
                    }
                }
            }
        }

        return space;
    }

    private static void Dump((int x, int y, int z) dim, int[,,] space)
    {
        System.Console.WriteLine("  x");
        for (int z = dim.z - 1; z >= 0; z--)
        {
            for (int x = 0; x < dim.x; x++)
            {
                int y = 0;
                char value = '.';
                while (y < dim.y)
                {
                    if (space[x, y, z] != 0)
                    {
                        value = (char)(space[x, y, z] + 64);
                        break;
                    }
                    y++;
                }
                System.Console.Write(value);
            }
            System.Console.WriteLine($" {z}");
        }

        System.Console.WriteLine("   y");
        for (int z = dim.z - 1; z >= 0; z--)
        {
            for (int y = 0; y < dim.y; y++)
            {
                int x = 0;
                char value = '.';
                while (x < dim.x)
                {
                    if (space[x, y, z] != 0)
                    {
                        value = (char)(space[x, y, z] + 64);
                        break;
                    }
                    x++;
                }
                System.Console.Write(value);
            }
            System.Console.WriteLine($" {z}");
        }
    }

    private static List<Brick> DropAll(List<Brick> bricks, Point3d dim)
    {
        var heights = new int[dim.x, dim.y];
        var droppedbricks = new List<Brick>();
        foreach (var brick in bricks)
        {
            var z = 0;
            for (int x = brick.Start.x; x <= brick.End.x; x++)
            {
                for (int y = brick.Start.y; y <= brick.End.y; y++)
                {
                    if (heights[x, y] > z)
                    {
                        z = heights[x, y];
                    }
                }
            }
            z++;

            var dropped = brick.DropTo(z);
            droppedbricks.Add(dropped);

            for (int x = brick.Start.x; x <= brick.End.x; x++)
            {
                for (int y = brick.Start.y; y <= brick.End.y; y++)
                {
                    heights[x, y] = dropped.End.z;
                }
            }
        }

        return droppedbricks;
    }

    record Brick(int Number, Point3d Start, Point3d End)
    {

        public static Brick Parse(string line, int number)
        {
            var (s, e) = line.Split('~', 2);

            Point3d start = Parse(s);
            Point3d end = Parse(e);

            if (end.x < start.x || end.y < start.y || end.z < start.z)
            {
                throw new NotImplementedException();
            }

            return new Brick(number, start, end);

            static Point3d Parse(string s)
            {
                var (x, y, z) = s.Split(',', 3).Select(int.Parse).ToArray();
                return (x, y, z);
            }

        }
        public IEnumerable<Brick> Supports { get; internal set; }
        public IEnumerable<Brick> SupportedBy { get; internal set; }

        internal Brick DropTo(int z)
        {
            if (z < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(z));
            }

            return new Brick(Number, (Start.x, Start.y, z), (End.x, End.y, z + End.z - Start.z));
        }

        public override string ToString()
        {
            return ((char)(Number + 64)).ToString();
        }
    }

    internal class BrickHeightComparer : IComparer<Brick>
    {
        int IComparer<Brick>.Compare(Brick lhs, Brick rhs)
        {
            return lhs.Start.z.CompareTo(rhs.Start.z);
        }
    }

    string sample = """
                    1,0,1~1,2,1
                    0,0,2~2,0,2
                    0,2,3~2,2,3
                    0,0,4~0,2,4
                    2,0,5~2,2,5
                    0,1,6~2,1,6
                    1,1,8~1,1,9
                    """;
}

