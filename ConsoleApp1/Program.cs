string pics = @"C:/Users/SPAC-O-5/source/repos/Cereal/Cereal/wwwroot/Images/Products/";
string csv = "C:/Users/SPAC-O-5/source/repos/Cereal/Cereal.csv";

int rowIndex = 0;
List<string> rowId = new List<string>();
List<string> csvNames = new List<string>();
foreach (var line in File.ReadLines(csv))
{
    if (rowIndex < 2)
    {
        rowIndex++;
        continue; // Skip header rows
    }

    string[] columns = line.Split(';');
    string csvName = columns[0];

    csvName = csvName.Replace(" ", "").Replace("'", "").Replace(",", "").Replace(".", "").ToLower();
    csvNames.Add(csvName);
}

int matchCount = 0;
int noMatchCount = 0;
int picId = 0;
foreach (var pic in Directory.GetFiles(pics))
{
    picId++;
    string picName = Path.GetFileNameWithoutExtension(pic);
    picName = picName.Trim();
    picName = picName.ToLower();
    picName = picName.Replace(" ", "").Replace("'", "").Replace(",", "").Replace(".", "");
    if (csvNames.Contains(picName))
    {
        matchCount++;
        //Console.WriteLine($"Match found: {picName}");
    }
    else
    {
        noMatchCount++;
        Console.WriteLine(csvNames[picId -1]);
        Console.WriteLine($"No match: {picName}");
        Console.WriteLine($"Pic id = {picId}\n");
    }
}
Console.WriteLine("\n");
Console.WriteLine(matchCount + " : " + noMatchCount);
