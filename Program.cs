using System.Reflection;

var lastPuzzleType = Assembly.GetEntryAssembly()
                             .GetTypes()
                             .Where(t => typeof(IPuzzle).IsAssignableFrom(t) && t.IsClass)
                             .OrderByDescending(t => int.Parse(t.Name[6..]))
                             .First();

var puzzle = (IPuzzle)Activator.CreateInstance(lastPuzzleType);


System.Console.WriteLine("Running {0}", lastPuzzleType.Name);
puzzle.Excute();