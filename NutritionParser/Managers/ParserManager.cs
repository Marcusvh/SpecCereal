using System.Collections.Generic;
using System.IO;

namespace Cereal.Managers
{
    public class ParserManager
    {
        public ParserManager() { }

        // Reads a CSV file and returns a list of string arrays (each array is a row)

        public List<string[]> ReadCsv()
        {
            string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.FullName;
            string filePath = Path.Combine(projectRoot, "Cereal(in).csv");

            var rows = new List<string[]>();
            foreach (string line in File.ReadLines(filePath))
            {
                var columns = line.Split(',');
                rows.Add(columns);
            }
            return rows;
        }
    }
}