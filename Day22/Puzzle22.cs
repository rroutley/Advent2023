using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using Point3d = (int x, int y, int z);

class Puzzle22 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day22/input.txt");
        lines = sample.Replace("\n", "").Split(['\r']);

        List<Brick> bricks = [];

        foreach (var line in lines)
        {
            var (s, e) = line.Split('~', 2);

            var brick = new Brick(Parse(s), Parse(e));
            bricks.Add(brick);

        }

        var maxX = bricks.Max(b => Math.Max(b.start.x, b.end.x)) + 1;
        var maxY = bricks.Max(b => Math.Max(b.start.y, b.end.y)) + 1;
        var maxZ = bricks.Max(b => Math.Max(b.start.z, b.end.z)) + 1;
        Point3d dim = (maxX, maxY, maxZ);

        System.Console.WriteLine(dim);

        bricks = Drop(bricks, dim);

        int[,,] space = Map(bricks, dim);

        Dump(dim, space);

        var total = lines.Count();

        System.Console.WriteLine($"Answer ={total}");
    }

    private static int[,,] Map(List<Brick> bricks, (int x, int y, int z) dim)
    {
        var space = new int[dim.x, dim.y, dim.z];

        for (int i = 0; i < bricks.Count; i++)
        {
            Brick brick = bricks[i];
            for (int x = brick.start.x; x <= brick.end.x; x++)
            {
                for (int y = brick.start.y; y <= brick.end.y; y++)
                {
                    for (int z = brick.start.z; z <= brick.end.z; z++)
                    {
                        space[x, y, z] = i + 1;
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
            for (int x = 0; x < dim.y; x++)
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

    private static List<Brick> Drop(List<Brick> bricks, (int x, int y, int z) dim)
    {
        var heights = new int[dim.x, dim.y];
        var droppedbricks = new List<Brick>();
        foreach (var brick in bricks)
        {
            var z = 0;
            for (int x = brick.start.x; x <= brick.end.x; x++)
            {
                for (int y = brick.start.y; y <= brick.end.y; y++)
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

            for (int x = brick.start.x; x <= brick.end.x; x++)
            {
                for (int y = brick.start.y; y <= brick.end.y; y++)
                {
                    heights[x, y] = z;
                }
            }
        }

        return droppedbricks;
    }

    record Brick(Point3d start, Point3d end)
    {
        internal Brick DropTo(int z)
        {
            return new Brick((start.x, start.y, z), (end.x, end.y, z + end.z - start.z));

        }
    }

    private Point3d Parse(string s)
    {
        var (x, y, z) = s.Split(',', 3).Select(int.Parse).ToArray();
        return (x, y, z);
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