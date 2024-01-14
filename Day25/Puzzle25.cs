

using System.Linq.Expressions;

class Puzzle25 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day25/input.txt");
        lines = sample.Replace("\n", "").Split(['\r']);

        var cx = new List<Connection>();

        foreach (var line in lines)
        {
            var (lhs, r) = line.Split(':', 2);
            var rhs = r.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var item in rhs)
            {
                cx.Add(new Connection(item, lhs));
                cx.Add(new Connection(lhs, item));
            }
        }

        var d = cx.OrderBy(c => c.From).ThenBy(c => c.To).ToList();

        var total = cx.Count();

        System.Console.WriteLine($"Answer ={total}");
    }

    record Connection(string From, string To);

    string sample = """
                    jqt: rhn xhk nvd
                    rsh: frs pzl lsr
                    xhk: hfx
                    cmg: qnr nvd lhk bvb
                    rhn: xhk bvb hfx
                    bvb: xhk hfx
                    pzl: lsr hfx nvd
                    qnr: nvd
                    ntq: jqt hfx bvb xhk
                    nvd: lhk
                    lsr: lhk
                    rzs: qnr cmg lsr rsh
                    frs: qnr lhk lsr
                    """;
}