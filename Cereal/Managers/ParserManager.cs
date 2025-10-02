using Cereal.Context;
using Cereal.Helpers;
using CerealLib;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Cereal.Managers
{
    public class ParserManager
    {
        CerealContext nutritionContext;
        public ParserManager(CerealContext context) 
        {
            nutritionContext = context;
        }

        public void ImagePathSetup() // a few adjustments might be needed, but overall works fine.
        {
            string basePath = @"Images/Products";
            string pics = @"C:/Users/SPAC-O-5/source/repos/Cereal/Cereal/wwwroot/Images/Products/";
            foreach (var file in Directory.GetFiles(pics))
            {
                string picName = Path.GetFileNameWithoutExtension(file);
                var product = nutritionContext.Nutritions.FirstOrDefault(n => n.Name.ToString() == picName);
                if (product != null)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    product.ImagePath = $"{basePath}/{product.Name}{fileInfo.Extension}";
                    nutritionContext.Nutritions.Update(product);
                }
            }

            nutritionContext.SaveChanges();
        }

        // Reads a CSV file and returns a list of string arrays
        public List<Nutrition> ReadCsv(string filePath, out List<int> skippedRowIndices)
        {
            skippedRowIndices = new List<int>(); // To track indices of skipped rows.
            List<Nutrition> addedNutritions = new List<Nutrition>();
            try
            {
                int rowIndex = 0;
                foreach (var line in File.ReadLines(filePath))
                {
                    if (rowIndex < 2)
                    {
                        rowIndex++;
                        continue; // Skip header rows
                    }

                    string[] columns = line.Split(';');

                    bool valid = ParserHelper.CheckForInvalidRows(columns);
                    if (!valid)
                    {
                        skippedRowIndices.Add(rowIndex);
                        rowIndex++;
                        continue; // Skip invalid entries
                    }
                    Nutrition nutrition = new ()
                    {
                        Name = columns[0],
                        Mfr = columns[1],
                        Type = columns[2],
                        Calories = int.Parse(columns[3]),
                        Protein = int.Parse(columns[4]),
                        Fat = int.Parse(columns[5]),
                        Sodium = int.Parse(columns[6]),
                        Fiber = float.Parse(columns[7]),
                        Carbo = float.Parse(columns[8]),
                        Sugars = int.Parse(columns[9]),
                        Potass = int.Parse(columns[10]),
                        Vitamins = int.Parse(columns[11]),
                        Shelf = int.Parse(columns[12]),
                        Weight = float.Parse(columns[13]),
                        Cups = float.Parse(columns[14]),
                        Rating = float.Parse(columns[15])
                    };
                    bool isDublicate = ParserHelper.CheckForDublicate(nutrition, nutritionContext);
                    if (isDublicate)
                    {
                        skippedRowIndices.Add(rowIndex);
                        rowIndex++;
                        continue; // Skip duplicate entries
                    }

                    addedNutritions.Add(nutrition);
                    nutritionContext.Nutritions.Add(nutrition);
                }

                nutritionContext.SaveChanges();
                return addedNutritions;
            } catch (IOException ex)
            {
                // Handle file I/O errors
                throw new Exception("Error reading the CSV file.", ex);
            }
            catch (FormatException ex)
            {
                // Handle data format errors
                throw new Exception("Data format error in the CSV file.", ex);
            }

        }

        public List<Nutrition> getAllProducts()
        {
            return nutritionContext.Nutritions.ToList();
        }

        public Nutrition GetSingleProduct(int id)
        {
            return nutritionContext.Nutritions.Find(id);
        }

        public Nutrition ParseProduct(Nutrition nutrition)
        {
            bool isDublicate = ParserHelper.CheckForDublicate(nutrition, nutritionContext);
            if (isDublicate)
            {
                return null; // Skip duplicate entries
            }

            nutritionContext.Add(nutrition);
            nutritionContext.SaveChanges();
            return nutrition;
        }

        public Nutrition DeleteProduct(int id)
        {
            Nutrition entity = nutritionContext.Nutritions.Find(id);
            if (entity != null)
            {
                nutritionContext.Nutritions.Remove(entity);
                nutritionContext.SaveChanges();
            }
            return entity;
        }

        public void DeleteAllProducts()
        {
            nutritionContext.Database.ExecuteSqlRaw("Truncate TABLE cereal.nutritions");
            nutritionContext.SaveChanges();
        }
        public Nutrition UpdateProduct(int id, Nutrition updatedNutrition, out string typeError)
        {
            typeError = "";
            Nutrition existingNutrition = nutritionContext.Nutritions.Find(id);
            if (existingNutrition == null)
            {
                typeError = "not found";
                return null; 
            }
            if(ParserHelper.CheckForDublicate(updatedNutrition, nutritionContext))
            {
                typeError = "duplicate";
                return null; 
            }
            if(!ParserHelper.CheckForInvalidRows([
                updatedNutrition.Name,
                updatedNutrition.Mfr,
                updatedNutrition.Type,
                updatedNutrition.Calories.ToString(),
                updatedNutrition.Protein.ToString(),
                updatedNutrition.Fat.ToString(),
                updatedNutrition.Sodium.ToString(),
                updatedNutrition.Fiber.ToString(),
                updatedNutrition.Carbo.ToString(),
                updatedNutrition.Sugars.ToString(),
                updatedNutrition.Potass.ToString(),
                updatedNutrition.Vitamins.ToString(),
                updatedNutrition.Shelf.ToString(),
                updatedNutrition.Weight.ToString(),
                updatedNutrition.Cups.ToString(),
                updatedNutrition.Rating.ToString()
            ])) // temp
            {
                typeError = "invalid";
                return null; 
            } 
            nutritionContext.Nutritions.Update(updatedNutrition);
            nutritionContext.SaveChanges();

            return updatedNutrition;
        }


        public List<Nutrition> GetFilteredProducts(string category, string value)
        {
            return nutritionContext.Nutritions
                .Where($"{category} == @0", value)
                .ToList();
        }
    }
}
