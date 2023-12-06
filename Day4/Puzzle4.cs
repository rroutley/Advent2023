
using System.Security.Cryptography.X509Certificates;

class Puzzle4 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day4/input.txt");
        //var lines = sample.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        var cards = new List<Card>();
        foreach (var line in lines)
        {
            var card = Card.Parse(line);
            cards.Add(card);
        }

        int total = 0;
        foreach (var card in cards)
        {
            //System.Console.WriteLine(card);
            var m = card.Matches();
            // System.Console.WriteLine($"card {card.Number} = {m}");
            if (m > 0)
                total += 1 << (m - 1);
        }

        System.Console.WriteLine("Answer = {0}", total);
        var x = cards.Count;

        for (int i = 0; i < cards.Count; i++)
        {
            var card = cards[i];
            var m = card.Matches();

            for (int j = 0; j < m; j++)
            {
                cards.Add(cards[card.Number + j]);

            }

        }


        System.Console.WriteLine("Part 2 = {0}", cards.Count);
    }



    public record Card(int Number, HashSet<int> Answers, HashSet<int> Selection)
    {

        public static Card Parse(string line)
        {
            (var p1, var p2) = line.Split(':', 2);

            var card = int.Parse(p1[5..]);

            (var p3, var p4) = p2.Split('|', 2);

            var answers = p3.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x));
            var selection = p4.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x));

            return new Card(card, new HashSet<int>(answers), new HashSet<int>(selection));
        }


        public int Matches()
        {
            return Selection.Intersect(Answers).Count();
        }
    };

    string sample = """
                    Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53
                    Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19
                    Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1
                    Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83
                    Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36
                    Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11
                    """;
}