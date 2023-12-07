using System.Text.RegularExpressions;

class Puzzle5 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day5/input.txt");
        //var lines = sample.Replace("\n", "").Split(new char[] { '\r' });

        (var seeds, var maps) = ParseLines(lines);

        var lowestSeed = long.MinValue;
        var lowest = long.MaxValue;

        foreach (var seed in seeds)
        {
            var type = "seed";
            var value = seed;
            while (true)
            {
                Console.Write($"{type} = {value}, ");
                if (type == "location")
                    break;

                var map = maps[type];
                value = map.Apply(value);
                type = map.Destiantion;
            }
            System.Console.WriteLine();

            if (value < lowest)
            {
                lowest = value;
                lowestSeed = seed;
            }
        }

        System.Console.WriteLine("Answer = {0}; seed = {1}", lowest, lowestSeed);
    }

    private static (List<long>, Dictionary<string, Map>) ParseLines(string[] lines)
    {
        var maps = new Dictionary<string, Map>();
        var seeds = new List<long>();
        string source = string.Empty, dest = string.Empty;
        var items = new List<MapItem>();
        foreach (var line in lines)
        {
            if (line.StartsWith("seeds:"))
            {
                seeds = line.Substring(6).Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToList();
            }
            else if (line.Contains("map:"))
            {
                var m = Regex.Match(line, @"(\w+)\-to\-(\w+)");
                source = m.Groups[1].Value;
                dest = m.Groups[2].Value;
            }
            else if (!string.IsNullOrEmpty(source))
            {
                if (line.Length > 0)
                {
                    items.Add(MapItem.Parse(line));
                }
                else
                {
                    maps.Add(source, new Map(source, dest, items));
                    source = string.Empty;
                    dest = string.Empty;
                    items = new List<MapItem>();
                }
            }
        }
        if (!string.IsNullOrEmpty(source))
        {
            maps.Add(source, new Map(source, dest, items));
        }

        return (seeds, maps);
    }

    record Map(string Source, string Destiantion, List<MapItem> Items)
    {
        internal long Apply(long value)
        {
            foreach (var item in Items)
            {
                if (item.TryApply(value, out var mappedValue))
                {
                    return mappedValue;
                }
            }
            return value;
        }
    }

    record MapItem(long Destiantion, long Source, long Length)
    {
        public static MapItem Parse(string line)
        {
            (long dest, long source, long len) = line.Split(' ', 3).Select(long.Parse).ToArray();

            return new MapItem(dest, source, len);
        }

        public bool TryApply(long value, out long mappedValue)
        {
            if (Source <= value && value < Source + Length)
            {
                mappedValue = Destiantion + value - Source;
                return true;
            }
            mappedValue = value;
            return false;
        }

    }



    string sample = """
                    seeds: 79 14 55 13

                    seed-to-soil map:
                    50 98 2
                    52 50 48

                    soil-to-fertilizer map:
                    0 15 37
                    37 52 2
                    39 0 15

                    fertilizer-to-water map:
                    49 53 8
                    0 11 42
                    42 0 7
                    57 7 4

                    water-to-light map:
                    88 18 7
                    18 25 70

                    light-to-temperature map:
                    45 77 23
                    81 45 19
                    68 64 13

                    temperature-to-humidity map:
                    0 69 1
                    1 0 69

                    humidity-to-location map:
                    60 56 37
                    56 93 4
                    """;
}