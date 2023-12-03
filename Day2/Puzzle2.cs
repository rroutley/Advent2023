using System;
using System.Text.RegularExpressions;
using Play = (int Red, int Blue, int Green);

class Puzzle2 : IPuzzle
{

    public void Excute()
    {

        List<Game> games = new List<Game>();

        var lines = File.ReadAllLines("Day2/input.txt");
        //var lines = sample.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var game = Game.Parse(line);

            //System.Console.WriteLine(game);

            games.Add(game);
        }


        var actual = (12, 13, 14);
        var total = 0;

        foreach (var game in games)
        {

            if (game.IsPossible(actual))
            {
                System.Console.WriteLine(game);
                total += game.Number;
            }
        }

        System.Console.WriteLine($"Answer Part 1 = {total}");

        total = 0;
        foreach (var game in games)
        {

            Play min = game.Minima();
            {
                int power = min.Red * min.Green * min.Blue;
                System.Console.WriteLine("{0}, {1}, {2}", game.Number, min, power);

                total += power;

            }
        }

        System.Console.WriteLine($"Answer Part 2 = {total}");

    }





    record Game(int Number, IEnumerable<Play> Plays)
    {
        public static Game Parse(string line)
        {
            (string p1, string p2) = line.Split(":").ToArray();

            int game = int.Parse(Regex.Match(p1, @"Game (\d+)").Groups[1].Value);

            var reveals = p2.Split(";");
            var samples = new List<Play>();

            foreach (var play in reveals)
            {
                Play sample = ParseReveal(play);

                samples.Add(sample);

            }

            return new Game(game, samples);

        }

        private static Play ParseReveal(string reveal)
        {
            var colors = reveal.Split(",");
            int r = 0, g = 0, b = 0;

            foreach (var color in colors)
            {
                var match = Regex.Match(color, @"(\d+) (\w+)");
                var c = match.Groups[2].Value;
                var n = int.Parse(match.Groups[1].Value);

                switch (c)
                {
                    case "red":
                        r = n;
                        break;
                    case "blue":
                        b = n;
                        break;
                    case "green":
                        g = n;
                        break;
                }
            }

            var sample = (r, g, b);
            return sample;
        }

        internal bool IsPossible(Play actual)
        {
            return Plays.All(p => p.Red <= actual.Red && p.Green <= actual.Green && p.Blue <= actual.Blue);
        }

        internal Play Minima()
        {
            int r = 0, g = 0, b = 0;
            foreach (var p in Plays)
            {
                r = Math.Max(r, p.Red);
                g = Math.Max(g, p.Green);
                b = Math.Max(b, p.Blue);
            }
            return (r, g, b);
        }
    };



    string sample = """
                    Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
                    Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
                    Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
                    Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
                    Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green
                    """;
}