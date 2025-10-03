using CerealLib;
using CerealLib.DTO;

namespace Cereal.Helpers
{
    public static class ControllerHelper
    {
        public static Nutrition MapNutrionDto(NutritionDTO dto)
        {
            Nutrition nutritionMapped = new Nutrition
            {
                Name = dto.Name,
                Mfr = dto.Mfr,
                Type = dto.Type,
                Calories = dto.Calories,
                Protein = dto.Protein,
                Fat = dto.Fat,
                Sodium = dto.Sodium,
                Fiber = dto.Fiber,
                Carbo = dto.Carbo,
                Sugars = dto.Sugars,
                Potass = dto.Potass,
                Vitamins = dto.Vitamins,
                Shelf = dto.Shelf,
                Weight = dto.Weight,
                Cups = dto.Cups,
                Rating = dto.Rating
            };
            return nutritionMapped;
        }
        public static string NormalizeImagePathString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            return input.Replace(" ", "").Replace("'", "").Replace(",", "").Replace(".", "").ToLower();
        }
    }
}
