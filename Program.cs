using System.Reflection;

var lastPuzzleType = Assembly.GetEntryAssembly()
                             .GetTypes()
                             .Where(t => typeof(IPuzzle).IsAssignableFrom(t) && t.IsClass)
                             .OrderByDescending(t => t.Name)
                             .First();

var puzzle = Activator.CreateInstance(lastPuzzleType) as IPuzzle;


System.Console.WriteLine("Running {0}", lastPuzzleType.Name);
puzzle.Excute();