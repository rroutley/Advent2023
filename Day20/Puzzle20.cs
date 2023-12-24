
using System.Collections.Concurrent;

class Puzzle20 : IPuzzle
{

    enum Pulse
    {
        NotSet,
        Low,
        High
    }

    public void Excute()
    {

        var lines = File.ReadAllLines("Day20/input.txt");
        //      lines = sample2.Replace("\n", "").Split(['\r']);

        var modules = ParseLines(lines);

        ConnectInputModules(modules);

        Part1(modules);


        Part2(modules,lines);

    }

    private static void Part1(Dictionary<string, Module> modules)
    {
        long highCount = 0;
        long lowCount = 0;

        int count = 1000;
        for (int i = 0; i < count; i++)
        {
            ButtonPress(modules, ref highCount, ref lowCount, i < 10);
        }
        var total = lowCount * highCount;

        System.Console.WriteLine($"Answer ={total}  = {highCount} * {lowCount}");
        System.Console.WriteLine(11687500);
    }


    private static void Part2(Dictionary<string, Module> modules, string[] lines)
    {
        List<long> counts = [];
        // RX Module has one input "bq"
        // bq is a conjunction with 4 inputs
        // find the count for each of bq's inputs and 
        // take the Least Common Multiple of these counts.

        var conMod = (ConjunctionModule)modules["bq"];
        foreach (var m in conMod.Inputs)
        {
            modules = ParseLines(lines);
            ConnectInputModules(modules);

            int item = CountUntilEnd(modules, m);
            counts.Add(item);

            System.Console.WriteLine($"{m} {item}");
        }

        long i = counts.Aggregate(Numerics.Lcm);

        System.Console.WriteLine($"Answer = {i}");

    }

    private static int CountUntilEnd(Dictionary<string, Module> modules, string endModule)
    {

        bool done = false;
        long highCount = 0;
        long lowCount = 0;

        modules[endModule] = new RxModule(endModule, () => { done = true; });
        int i = 0;
        while (!done)
        {
            i++;
            ButtonPress(modules, ref highCount, ref lowCount, false);
        }

        return i;
    }

    private static void ButtonPress(Dictionary<string, Module> modules, ref long highCount, ref long lowCount, bool debug)
    {
        var pulseQueue = new Queue<string>();
        pulseQueue.Enqueue("button");
        while (pulseQueue.Count > 0)
        {
            var name = pulseQueue.Dequeue();
            var module = modules[name];

            var pulse = module.Receive();

            foreach (var output in module.Outputs)
            {
                if (pulse == Pulse.High) highCount++;
                if (pulse == Pulse.Low) lowCount++;
                if (debug)
                {
                    System.Console.WriteLine("{0} -{1}-> {2}", name, pulse, output);
                }

                if (modules.TryGetValue(output, out var outputModule))
                {
                    if (outputModule.SetState(name, pulse))
                    {
                        pulseQueue.Enqueue(output);
                    }
                }

            }

        }
    }

    private static void ConnectInputModules(Dictionary<string, Module> modules)
    {
        var inputs = from m in modules.Values
                     from o in m.Outputs
                     let item = (o, m)
                     group item by item.o into g
                     select new
                     {
                         Input = g.Key,
                         Modules = g.Select(x => x.m)
                     };

        foreach (var input in inputs)
        {
            if (modules.TryGetValue(input.Input, out var module) && module is ConjunctionModule c)
            {
                foreach (var m in input.Modules)
                {
                    c.AddInput(m.Name);
                }
            }
        }
    }

    private static Dictionary<string, Module> ParseLines(string[] lines)
    {
        var modules = new Dictionary<string, Module>();

        var button = new ButtonModule("button", ["broadcaster"]);
        modules.Add(button.Name, button);

        var ouptutModule = new OutputModule("output", []);
        modules.Add(ouptutModule.Name, ouptutModule);

        foreach (var line in lines)
        {
            (var from, var to) = line.Split(" -> ", 2);

            var args = to.Split(", ");

            if (from == "broadcaster")
            {
                modules.Add(from, new BroadcastModule(from, [.. args]));
            }
            else if (from.StartsWith('%'))
            {
                var name = from[1..];
                modules.Add(name, new FlipFlopModule(name, [.. args]));
            }
            else if (from.StartsWith('&'))
            {
                var name = from[1..];
                modules.Add(name, new ConjunctionModule(name, [.. args]));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        return modules;
    }

    abstract record Module(string Name, string[] Outputs)
    {
        public abstract Pulse Receive();
        public abstract bool SetState(string input, Pulse value);
    };

    record FlipFlopModule(string Name, string[] Outputs) : Module(Name, Outputs)
    {
        private bool Value;

        public override Pulse Receive()
        {
            return Value ? Pulse.High : Pulse.Low;
        }

        public override bool SetState(string input, Pulse pulse)
        {
            if (pulse == Pulse.High)
            {
                return false;
            }

            Value = !Value;
            return true;
        }
    }

    record ConjunctionModule(string Name, string[] Outputs) : Module(Name, Outputs)
    {

        private Dictionary<string, Pulse> inputs = new Dictionary<string, Pulse>();

        public IEnumerable<string> Inputs => inputs.Keys;
        public void AddInput(string name)
        {
            inputs.TryAdd(name, Pulse.Low);
        }

        public override Pulse Receive()
        {
            var value = inputs.Values.All(i => i == Pulse.High);
            return value ? Pulse.Low : Pulse.High;
        }

        public override bool SetState(string input, Pulse value)
        {
            inputs[input] = value;
            return true;
        }
    }

    record BroadcastModule(string Name, string[] Outputs) : Module(Name, Outputs)
    {

        private Pulse value;
        public override Pulse Receive()
        {
            return this.value;
        }

        public override bool SetState(string input, Pulse pulse)
        {
            this.value = pulse;
            return true;
        }
    }

    record ButtonModule(string Name, string[] Outputs) : Module(Name, Outputs)
    {
        public override Pulse Receive()
        {
            return Pulse.Low;
        }

        public override bool SetState(string input, Pulse value)
        {
            return true;
        }
    }


    record OutputModule(string Name, string[] Outputs) : Module(Name, Outputs)
    {

        private Pulse value;
        public override Pulse Receive()
        {
            return value;
        }

        public override bool SetState(string input, Pulse pulse)
        {
            value = pulse;
            return true;
        }
    }

    record RxModule(string Name, Action done) : Module(Name, [])
    {
        public override Pulse Receive()
        {
            return Pulse.NotSet;
        }

        public override bool SetState(string input, Pulse value)
        {
            if (value == Pulse.Low)
            {
                done();
            }
            return false;
        }
    }

    string sample = """
                    broadcaster -> a, b, c
                    %a -> b
                    %b -> c
                    %c -> inv
                    &inv -> a
                    """;

    string sample2 = """
                    broadcaster -> a
                    %a -> inv, con
                    &inv -> b
                    %b -> con
                    &con -> output
                    """;
}