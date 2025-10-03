using Cereal.Context;
using Cereal.Helpers;
using CerealLib;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Cereal.Managers
{
    // should have an interface, but for the sake of time, skipping it.
    // should have split this class into smaller classes, but for the sake of time, skipping it.
    public class ParserManager
    {
        CerealContext nutritionContext;
        public ParserManager(CerealContext context) 
        {
            nutritionContext = context;
        }
        /// <summary>
        /// Reads nutrition data from a CSV file and adds valid entries to the database.
        /// </summary>
        /// <param name="filePath">The full path to the CSV file containing nutrition data.</param>
        /// <param name="skippedRowIndices">
        /// An output list of row indices that were skipped due to invalid format or duplication.
        /// </param>
        /// <returns>
        /// A list of <see cref="Nutrition"/> objects that were successfully parsed and added to the database.
        /// </returns>
        /// <remarks>
        /// This method skips the first two header rows, validates each row for correct formatting,
        /// checks for duplicates, and tracks any skipped rows. After processing, it saves the valid entries
        /// to the database and sets image paths for each nutrition item.
        /// </remarks>
        /// <exception cref="Exception">
        /// Thrown when the file cannot be read or contains improperly formatted data.
        /// </exception>
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
                        Rating = float.Parse(columns[15]),
                        ImagePath = "" // to be set later
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
                ImagePathSetup(); // call to set image paths
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
        /// <summary>
        /// Updates an existing nutrition product in the database by its ID.
        /// </summary>
        /// <param name="id">The ID of the nutrition product to update.</param>
        /// <param name="updatedNutrition">The updated <see cref="Nutrition"/> object containing new values.</param>
        /// <param name="typeError">
        /// An output string indicating the reason for failure, if any. Possible values:
        /// <list type="bullet">
        /// <item><c>"not found"</c> – No product with the given ID exists.</item>
        /// <item><c>"duplicate"</c> – The updated product matches an existing entry.</item>
        /// <item><c>"invalid"</c> – The updated product contains invalid data.</item>
        /// </list>
        /// </param>
        /// <returns>
        /// The updated <see cref="Nutrition"/> object if successful; otherwise, <c>null</c>.
        /// </returns>
        /// <remarks>
        /// This method performs validation checks for existence, duplication, and data integrity before updating.
        /// If any check fails, the update is aborted and an appropriate error message is returned via <c>typeError</c>.
        /// </remarks>
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
            ]))
            {
                typeError = "invalid";
                return null; 
            } 
            nutritionContext.Nutritions.Update(updatedNutrition);
            nutritionContext.SaveChanges();

            return updatedNutrition;
        }

        public List<Nutrition> GetFilteredProducts(string category, string value, string sorting)
        {
            string[] validProperties = new[] { "Name", "Type", "Calories", "Protein", "Fat", "Rating" }; // Add other or maybe all properties.
            if (!validProperties.Contains(category)) category = "Name";

            IQueryable<Nutrition> query = nutritionContext.Nutritions;

            // Apply filtering only if category and value are provided
            if (!string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(value))
            {
                query = query.Where($"{category} == @0", value);
            }

            // Apply sorting
            string sortExpression = "";
            if (!string.IsNullOrEmpty(sorting)) // validation of sorting parameter
            {
                string[] split = sorting.Split('_');
                if (!validProperties.Contains(split[0])) split[0] = "Name";
                if (split.Length != 2 || (split[1] != "asc" && split[1] != "desc"))
                {
                    split = new[] { "Name", "asc" }; // default sorting
                }
                sortExpression = $"{split[0]} {split[1]}";
                query = query.OrderBy(sortExpression);
            }

            return query.ToList();
        }

        /// <summary>
        /// Assigns image file paths to nutrition products by matching image filenames to product names.
        /// </summary>
        /// <remarks>
        /// This method scans the <c>wwwroot/Images/Products</c> directory for image files and attempts to match each file
        /// to a nutrition product in the database by normalizing both the product name and the image filename.
        /// Normalization removes spaces, commas, dots, apostrophes, and applies lowercase conversion to ensure consistent matching.
        /// If a match is found, the product's <c>ImagePath</c> property is updated accordingly.
        /// </remarks>
        public void ImagePathSetup()
        {
            string basePath = @"wwwroot/Images/Products";
            foreach (var file in Directory.GetFiles(basePath))
            {
                string picName = Path.GetFileNameWithoutExtension(file);
                var product = nutritionContext.Nutritions.FirstOrDefault(n => ControllerHelper.NormalizeImagePathString(n.Name) == ControllerHelper.NormalizeImagePathString(picName)); // Need to normalize the product names and image names, so they match. differenses in spaces, commas, dots, apostrophes, and case.
                if (product != null)
                {
                    FileInfo fileInfo = new FileInfo(file); // to get the file extension, including the dot
                    product.ImagePath = $"{basePath}/{picName}{fileInfo.Extension}";
                    nutritionContext.Nutritions.Update(product);
                }
            }

            nutritionContext.SaveChanges();
        }
    }
}
