


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
        
        System.Console.WriteLine($"Answer ={total}");
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