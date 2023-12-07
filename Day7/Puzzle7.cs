
class Puzzle7 : IPuzzle
{


    public void Excute()
    {

        var lines = File.ReadAllLines("Day7/input.txt");
        // lines = sample.Replace("\n", "").Split(['\r']);

        var hands = new List<(Hand, int)>();
        foreach (var line in lines)
        {
            (string h, string b) = line.Split(' ', 2);

            var hand = new Hand(h.ToArray());
            var bid = int.Parse(b);

            System.Console.WriteLine($"{hand} = {bid}");

            hands.Add((hand, bid));

        }


        var ordered = hands.OrderBy(h => h.Item1, new HandComparer());

        foreach (var hand in ordered)
        {
            System.Console.WriteLine(hand);
        }

        var answer = ordered.Select((h, i) => (i + 1) * h.Item2).Sum();

        System.Console.WriteLine(answer);
    }

    class HandComparer : IComparer<Hand>
    {
        public int Compare(Hand? x, Hand? y)
        {
            if (y == null || x == null) return -1;

            var ts = x.Strength();
            var os = y.Strength();

            if (ts != os)
            {
                return -ts.CompareTo(os);
            }

            for (int i = 0; i < 5; i++)
            {
                var to = Hand.Ordering.IndexOf(x.Cards[i]);
                var oo = Hand.Ordering.IndexOf(y.Cards[i]);
                if (to != oo)
                {
                    return to.CompareTo(oo);
                }

            }
            return 0;

        }
    }

    record Hand(char[] Cards)
    {
        internal static readonly List<char> Ordering = ['2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A'];
        public override string ToString()
        {
            return string.Join("", Cards);
        }

        public enum Type
        {
            FiveOfAKind,
            FourOfAKind,
            FullHouse,
            ThreeOfAKind,
            TwoPair,
            OnePair,
            HighCard,
        }

        public Type Strength()
        {
            var rating = Cards.Select(c => Ordering.IndexOf(c))
                         .OrderByDescending(c => c)
                         .GroupBy(c => c)
                         .Select(g => new { Card = Ordering[g.Key], Ordering = g, Count = g.Count() })
                         .OrderByDescending(g => g.Count)
                         .ToArray();

            Type kind;

            if (rating[0].Count == 5)
            {
                // Five of a Kind
                kind = Type.FiveOfAKind;
            }
            else if (rating[0].Count == 4)
            {
                //Four of a Kind
                kind = Type.FourOfAKind;
            }
            else if (rating[0].Count == 3 && rating[1].Count == 2)
            {
                // Full House
                kind = Type.FullHouse;

            }
            else if (rating[0].Count == 3)
            {
                // Three Of a kind
                kind = Type.ThreeOfAKind;
            }
            else if (rating[0].Count == 2 && rating[1].Count == 2)
            {
                // Two Pair
                kind = Type.TwoPair;
            }
            else if (rating[0].Count == 2)
            {
                // One Pair
                kind = Type.OnePair;
            }
            else
            {
                // High Card
                kind = Type.HighCard;
            }

            return kind;

        }


    };

    string sample = """
                    32T3K 765
                    T55J5 684
                    KK677 28
                    KTJJT 220
                    QQQJA 483
                    """;
}