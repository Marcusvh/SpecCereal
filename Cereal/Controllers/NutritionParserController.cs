using Cereal.ApiResponses;
using Cereal.Helpers;
using Cereal.Managers;
using CerealLib;
using CerealLib.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cereal.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize] // Require authentication for all actions in this controller
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
	public class NutritionParserController : ControllerBase
    {
        private readonly ParserManager parserManager;
        public NutritionParserController(ParserManager parser)
        {
            parserManager = parser;
        }

        [HttpPost("UploadCSV")]
        [ProducesResponseType(typeof(CsvUploadResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post([FromBody] string path)
        {
            if (string.IsNullOrEmpty(path))
                return BadRequest("File path is null or empty.");

            if (!System.IO.File.Exists(path))
                return BadRequest("File does not exist at the specified path.");

            List<int> skippedRowIndices;
            List<Nutrition> addedNutritions = parserManager.ReadCsv(path, out skippedRowIndices);

            if (addedNutritions.Count == 0)
            {
                return Conflict("All rows were duplicates. No new data added.");
            }

            CsvUploadResponse result = new CsvUploadResponse
            {
                TotalRows = addedNutritions.Count + skippedRowIndices.Count,
                AddedRows = addedNutritions.Count,
                SkippedRowIndices = skippedRowIndices,
                Message = $"Upload complete. {addedNutritions.Count} rows added, {skippedRowIndices.Count} rows skipped."
            };

            return Created("/api/v1/NutritionParser", result);
        }
        [HttpPost("UploadProduct")]
        [ProducesResponseType(typeof(Nutrition), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PostProduct([FromBody] NutritionDTO nutrition)
        {
            if (nutrition == null)
                return BadRequest("Product data is null or empty.");
            
            Nutrition nutritionInput = ControllerHelper.MapNutrionDto(nutrition);

            Nutrition resp = parserManager.ParseProduct(nutritionInput);
            if (resp == null)
                return BadRequest("Invalid product data.");

            return Created($"/api/v1/NutritionParser/{resp.Cereal_Id}", resp);
        }

       
        // DELETE api/<NutritionParserController>/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(int id)
        {
            if(id <= 0)
            {
                return BadRequest("The id cannot be 0 or less");
            }
            
            Nutrition productToDelete = parserManager.DeleteProduct(id);
            if (productToDelete == null)
            {
                return NotFound("No product found with that id");
            }

            return NoContent();
        }
        [HttpDelete("DeleteAll")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteAll()
        {
            try
            {
                parserManager.DeleteAllProducts();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting all products: {ex.Message}");
            }
        }
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Nutrition), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PutUpdate(int id, [FromBody] NutritionDTO nutrition)
        {
            if (id <= 0)
            {
                return BadRequest("The id cannot be 0 or less");
            }
            Nutrition nutritionInput = ControllerHelper.MapNutrionDto(nutrition);
            string typeError;
            Nutrition resp = parserManager.UpdateProduct(id, nutritionInput, out typeError);

            if (typeError == "not found")
            {
                return NotFound("No product found with that id");
            }
            if(typeError == "invalid")
            {
                return BadRequest("Invalid product data.");
			}
            if(typeError == "duplicate")
            {
                return Conflict("Update would result in a duplicate product.");
			}

			return Ok(resp);
        }
    }
}
