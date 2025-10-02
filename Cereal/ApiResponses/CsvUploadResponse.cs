namespace Cereal.ApiResponses
{
    public class CsvUploadResponse
    {
        public int TotalRows { get; set; }
        public int AddedRows { get; set; }
        public List<int> SkippedRowIndices { get; set; }
        public string Message { get; set; }
    }
}
