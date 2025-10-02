using Cereal.Context;
using CerealLib;

namespace Cereal.Helpers
{
    public static class ParserHelper
    {
        public static bool CheckForInvalidRows(string[] columns)
        {
            bool valid =
                int.TryParse(columns[3], out int calories) &&
                int.TryParse(columns[4], out int protein) &&
                int.TryParse(columns[5], out int fat) &&
                int.TryParse(columns[6], out int sodium) &&
                float.TryParse(columns[7], out float fiber) &&
                float.TryParse(columns[8], out float carbo) &&
                int.TryParse(columns[9], out int sugars) &&
                int.TryParse(columns[10], out int potass) &&
                int.TryParse(columns[11], out int vitamins) &&
                int.TryParse(columns[12], out int shelf) &&
                float.TryParse(columns[13], out float weight) &&
                float.TryParse(columns[14], out float cups) &&
                float.TryParse(columns[15], out float rating);

            return valid;
        }
        public static bool CheckForDublicate(Nutrition nutrition, CerealContext nutritionContext)
        {
            return nutritionContext.Nutritions.Any(n =>
                n.Name == nutrition.Name &&
                n.Mfr == nutrition.Mfr &&
                n.Type == nutrition.Type
            );
        
        }
    }
}
