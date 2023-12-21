using System.Text.RegularExpressions;

class Puzzle19 : IPuzzle
{

    public void Excute()
    {

        var lines = File.ReadAllLines("Day19/input.txt");
    //    lines = sample.Replace("\n", "").Split(['\r']);

        int section = 0;
        var workflows = new Dictionary<string, Workflow>();
        var parts = new List<Part>();
        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                section++;
                continue;
            }

            if (section == 0)
            {
                int length = line.IndexOf('{');
                var name = line.Substring(0, length);
                var flowList = line[(length + 1)..^1].Split(',');

                var statements = new List<Statement>();
                foreach (var flow in flowList)
                {

                    Statement statement;
                    var colon = flow.IndexOf(':');
                    if (colon < 0)
                    {
                        statement = ParseStatement(flow);
                    }
                    else
                    {
                        var expression = ParseExpression(flow[0..colon]);
                        Statement body = ParseStatement(flow[(colon + 1)..]);
                        statement = new ConditionalStatement(expression, body);
                    }
                    statements.Add(statement);
                }

                var workflow = new Workflow(name, statements);
                workflows.Add(name, workflow);
                System.Console.WriteLine(workflow);

            }
            if (section == 1)
            {
                var match = Regex.Match(line, @"\{x=(\d+),m=(\d+),a=(\d+),s=(\d+)\}");
                var part = new Part(int.Parse(match.Groups[1].Value),
                                    int.Parse(match.Groups[2].Value),
                                    int.Parse(match.Groups[3].Value),
                                    int.Parse(match.Groups[4].Value));

                parts.Add(part);
                System.Console.WriteLine(part);
            }
        }



        var total = lines.Count();

        System.Console.WriteLine($"Answer ={total}");
    }

    private Expression ParseExpression(string v)
    {
        var match = Regex.Match(v, @"(\w+)([<>])(\d+)");
        return new ConditionalExpression(match.Groups[1].Value, match.Groups[2].Value, int.Parse(match.Groups[3].Value));
    }

    private Statement ParseStatement(string v)
    {
        return v switch
        {
            "A" => new AcceptStatement(),
            "R" => new RejectStatement(),
            _ => new GotoStatement(v)
        };
    }

    record Part(int X, int M, int A, int S);
    record Workflow(string name, IEnumerable<Statement> Rules);

    record Statement();
    record Expression();

    record TrueExpression() : Expression;

    record ConditionalStatement(Expression predicate, Statement body) : Statement;
    record ConditionalExpression(string variable, string op, int value) : Expression;

    record GotoStatement(string label) : Statement;
    record AcceptStatement() : Statement;
    record RejectStatement() : Statement;

    string sample = """
                    px{a<2006:qkq,m>2090:A,rfg}
                    pv{a>1716:R,A}
                    lnx{m>1548:A,A}
                    rfg{s<537:gd,x>2440:R,A}
                    qs{s>3448:A,lnx}
                    qkq{x<1416:A,crn}
                    crn{x>2662:A,R}
                    in{s<1351:px,qqz}
                    qqz{s>2770:qs,m<1801:hdj,R}
                    gd{a>3333:R,R}
                    hdj{m>838:A,pv}

                    {x=787,m=2655,a=1222,s=2876}
                    {x=1679,m=44,a=2067,s=496}
                    {x=2036,m=264,a=79,s=2244}
                    {x=2461,m=1339,a=466,s=291}
                    {x=2127,m=1623,a=2188,s=1013}
                    """;
}