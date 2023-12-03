using System.Security.Cryptography.X509Certificates;

class Puzzle3 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day3/input.txt");
        //var lines = sample.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        var entries = FindAllNumbers(lines);
        foreach (var entry in entries)
        {
            System.Console.WriteLine(entry);
        }

        HashSet<Entry> parts = new HashSet<Entry>();

        int l = 0;
        foreach (var line in lines)
        {
            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (!(char.IsNumber(c) || c == '.'))
                {
                    System.Console.WriteLine($"{c} @ ({i},{l})");

                    foreach (var entry in entries)
                    {
                        if (entry.IsAdjacentTo(i, l))
                        {
                            parts.Add(entry);
                            System.Console.WriteLine(entry);
                        }
                    }
                }

            }
            l++;
        }

        var total = 0;
        foreach (var part in parts)
        {
            total += part.Value;
        }

        System.Console.WriteLine("Answer = {0}", total);

    }

    private IEnumerable<Entry> FindAllNumbers(string[] lines)
    {
        int l = 0;
        foreach (var line in lines)
        {
            int start = -1, end = -1;
            int value;
            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (char.IsNumber(c))
                {
                    if (start < 0)
                    {
                        start = i;
                    }
                }
                else
                {
                    if (start >= 0)
                    {
                        end = i;
                        value = int.Parse(line[start..end]);
                        var entry = new Entry(value, l, start, end);
                        start = -1;
                        yield return entry;
                    }
                }
            }

            if (start >= 0)
            {
                end = line.Length;
                value = int.Parse(line[start..end]);
                var entry = new Entry(value, l, start, end);
                start = -1;
                yield return entry;
            }

            l++;

        }
    }

    record Entry(int Value, int Line, int Start, int End)
    {
        public bool IsAdjacentTo(int col, int line)
        {
            if (Line - 1 <= line && line <= Line + 1)
            {
                if (Start - 1 <= col && col < End + 1)
                {
                    return true;
                }
            }
            return false;
        }
    };

    string sample = """
                    467..114..
                    ...*......
                    ..35..633.
                    ......#...
                    617*......
                    .....+.58.
                    ..592.....
                    ......755.
                    ...$.*....
                    .664.598..
                    """;
}