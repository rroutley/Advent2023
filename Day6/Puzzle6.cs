
class Puzzle6 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day6/input.txt");
        //lines = sample.Replace("\n", "").Split(new char[] { '\r' });

        lines[0] = lines[0].Replace(" ", "");
        lines[1] = lines[1].Replace(" ", "");

        var times = lines[0].Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
        var distances = lines[1].Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

        var inputs = times.Zip(distances);


        int result = 1;
        foreach (var input in inputs)
        {

            result *= Test(input);

        }
        System.Console.WriteLine("Answer = {0}", result);
    }

    private int Test((long Time, long Distance) input)
    {
        int x = 0;
        for (int t = 0; t < input.Time; t++)
        {
            long speed = t;
            long distance = (input.Time - t) * speed;
            if (distance > input.Distance)
            {
                x++;
            }
        }
        System.Console.WriteLine("{0} = {1}", input, x);
        return x;

    }

    string sample = """
                    Time:      7  15   30
                    Distance:  9  40  200
                    """;
}