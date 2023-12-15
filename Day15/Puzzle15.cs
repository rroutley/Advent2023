using System.Text;

class Puzzle15 : IPuzzle
{

    public void Excute()
    {

        var line = File.ReadAllText("Day15/input.txt");
        //line = sample;

        var values = line.Split(',');

        var total = 0;
        foreach (var value in values)
        {
            total += Hash(value);
        }

        System.Console.WriteLine($"Answer Part 1 ={total}");


        var hashMap = new List<Lens>[256];

        foreach (var value in values)
        {
            var lens = Lens.Parse(value);

            var boxIndex = Hash(lens.Label);

        //    System.Console.WriteLine($"{value} {lens} {boxIndex}");

            var box = hashMap[boxIndex] ??= [];
            if (lens.Operation == "-")
            {

                for (int i = 0; i < box.Count; i++)
                {
                    if (box[i].Label == lens.Label)
                    {
                        box.RemoveAt(i);
                        break;
                    }
                }
            }
            else
            {
                bool found = false;
                for (int i = 0; i < box.Count; i++)
                {
                    if (box[i].Label == lens.Label)
                    {
                        box[i] = lens;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    box.Add(lens);
                }
            }

        }


        var score = 0;
        for (int i = 0; i < hashMap.Length; i++)
        {
            if (hashMap[i] != null && hashMap[i].Count > 0)
            {
                System.Console.Write(i);
                for (int j = 0; j < hashMap[i].Count; j++)
                {
                    Lens item = hashMap[i][j];
                    System.Console.Write($" {item.Label} {item.FocalLength}");
                    score += (i + 1) * (j + 1) * item.FocalLength.Value;
                }
                System.Console.WriteLine();
            }
        }
        System.Console.WriteLine($"Part 2 = {score}");

    }


    record Lens(string Label, string Operation, int? FocalLength)
    {
        internal static Lens Parse(string value)
        {
            if (value.Contains('-'))
            {
                return new Lens(value[..^1], "-", null);
            }
            else
            {
                var x = value.IndexOf('=');
                return new Lens(value[..x], "=", int.Parse(value[(x + 1)..]));
            }
        }


    }

    private int Hash(string value)
    {
        var chars = Encoding.ASCII.GetBytes(value);

        var hash = 0;
        foreach (var c in chars)
        {
            hash = ((hash + c) * 17) % 256;
        }
        return hash;
    }

    string sample = "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7";
}