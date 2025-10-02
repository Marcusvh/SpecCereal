
    var rows = new List<string[]>();
    foreach (var line in File.ReadLines("Cereal(in).csv"))
    {
        // Simple split by comma, does not handle quoted commas
        var columns = line.Split(';');
        rows.Add(columns);
    Console.WriteLine(columns[0]);
    }
    