
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
            System.Console.WriteLine($"{hand.Item1} {hand.Item1.Strength()} {hand.Item2} ");
        }

        var answer = ordered.Select((h, i) => (i + 1) * h.Item2).Sum();

        System.Console.WriteLine(answer);
    }

    class HandComparer : IComparer<Hand>
    {
        public int Compare(Hand? x, Hand? y)
        {
            if (y == null || x == null) return -1;

            var xStrength = x.Strength();
            var yStrength = y.Strength();

            if (xStrength != yStrength)
            {
                return xStrength.CompareTo(yStrength);
            }

            for (int i = 0; i < 5; i++)
            {
                var xRank = Hand.Ordering.IndexOf(x.Cards[i]);
                var yRank = Hand.Ordering.IndexOf(y.Cards[i]);
                if (xRank != yRank)
                {
                    return xRank.CompareTo(yRank);
                }

            }
            return 0;

        }
    }

    record Hand(char[] Cards)
    {
        // Part 1 Ordering
        // internal static readonly List<char> Ordering = ['2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A'];

        // Part 2 Ordering
        internal static readonly List<char> Ordering = ['J', '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'Q', 'K', 'A'];
        public override string ToString()
        {
            return string.Join("", Cards);
        }

        public enum Type
        {
            HighCard,
            OnePair,
            TwoPair,
            ThreeOfAKind,
            FullHouse,
            FourOfAKind,
            FiveOfAKind,
        }

        private Type? _strength;

        public Type Strength()
        {
            _strength ??= RecursiveStrength(Cards, 0);

            return _strength.Value;
        }

        private Type RecursiveStrength(char[] cards, int start)
        {
            var maxStrength = Strength(cards);
            // Part 1
            //return maxStrength;  

            for (int i = start; i < 5; i++)
            {
                if (cards[i] == 'J')
                {
                    var cardsClone = new char[5];
                    cards.CopyTo(cardsClone, 0);

                    // replace J (= Joker) with ever other possible card and see if this improves the strength of the hand
                    // j=0 case not needed as this the ordering of the Joker card.
                    for (int j = 1; j < Ordering.Count; j++)
                    {
                        cardsClone[i] = Ordering[j];

                        var strength = RecursiveStrength(cardsClone, start + 1);
                        if (strength > maxStrength)
                        {
                            maxStrength = strength;
                        }
                    }
                }
            }

            return maxStrength;
        }

        private Type Strength(char[] cards)
        {
            var rating = cards.Select(c => Ordering.IndexOf(c))
                         .OrderByDescending(c => c)
                         .GroupBy(c => c)
                         .Select(g => new { Card = Ordering[g.Key], Ordering = g, Count = g.Count() })
                         .OrderByDescending(g => g.Count)
                         .ToArray();


            var kind = rating[0].Count switch
            {
                5 => Type.FiveOfAKind,
                4 => Type.FourOfAKind,
                3 when rating[1].Count == 2 => Type.FullHouse,
                3 => Type.ThreeOfAKind,
                2 when rating[1].Count == 2 => Type.TwoPair,
                2 => Type.OnePair,
                _ => Type.HighCard,
            };
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