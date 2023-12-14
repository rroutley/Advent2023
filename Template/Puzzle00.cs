

class Puzzle00 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day00/input.txt");
        lines = sample.Replace("\n", "").Split(['\r']);
        var total = lines.Count();

        System.Console.WriteLine($"Answer ={total}");
    }


    string sample = """

                    """;
}