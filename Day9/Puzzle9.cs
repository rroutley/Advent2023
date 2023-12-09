using System.Text.RegularExpressions;

class Puzzle9 : IPuzzle
{
    public void Excute()
    {

        var lines = File.ReadAllLines("Day9/input.txt");
        //     lines = sample.Replace("\n", "").Split(['\r']);


        var total = 0;
        foreach (var line in lines)
        {
            var history = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            var next = history[^1] + PredictNext(history);

            total += next;
            System.Console.WriteLine(next);

        }

        System.Console.WriteLine("Answer = {0}", total);


        total = 0;
        foreach (var line in lines)
        {
            var history = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            var next = history[0] - PredictPrevious(history);

            total += next;
            System.Console.WriteLine(next);

        }
        System.Console.WriteLine("Answer = {0}", total);

    }

    private int PredictNext(int[] history)
    {
        if (history.All(h => h == 0))
        {
            return 0;
        }

        int[] differences = CalcDifferences(history);

        var value = PredictNext(differences);

        return differences[^1] + value;
    }

    private static int[] CalcDifferences(int[] history)
    {
        var differences = new int[history.Length - 1];
        for (int i = 0; i < history.Length - 1; i++)
        {
            differences[i] = history[i + 1] - history[i];
        }

        return differences;
    }

    private int PredictPrevious(int[] history)
    {
        if (history.All(h => h == 0))
        {
            return 0;
        }

        int[] differences = CalcDifferences(history);

        var value = PredictPrevious(differences);

        return differences[0] - value;
    }

    string sample = """
                    0 3 6 9 12 15
                    1 3 6 10 15 21
                    10 13 16 21 30 45
                    """;
}