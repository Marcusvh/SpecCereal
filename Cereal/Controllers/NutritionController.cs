using Cereal.Managers;
using CerealLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cereal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous] // Allow anonymous (not logged in) access to all actions in this controller
    public class NutritionController : ControllerBase
    {
        private readonly ParserManager parserManager;
        public NutritionController(ParserManager parser)
        {
            parserManager = parser;
        }
        // GET: api/<NutritionParserController>
        [HttpGet]
        [ProducesResponseType(typeof(List<Nutrition>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll()
        {
            List<Nutrition> products = parserManager.getAllProducts();

            if (products == null)
            {
                return NotFound();
            }
            if (products.Count == 0)
            {
                return NoContent();
            }

            return Ok(products);
        }

        // GET api/<NutritionParserController>/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Nutrition), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get(int id)
        {
            Nutrition product = parserManager.GetSingleProduct(id);

            if (product == null)
            {
                return NotFound();
            }
            if (string.IsNullOrWhiteSpace(product.Name)) // check if a property is empty, indicating no content
            {
                return NoContent();
            }

            return Ok(product);
        }

        [HttpGet("products")]
        [ProducesResponseType(typeof(List<Nutrition>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetFilter([FromQuery] string? category, string? value, string sorting)
        {
            List<Nutrition> products = parserManager.GetFilteredProducts(category, value, sorting);
            if (products == null)
            {
                return NotFound();
            }
            if (products.Count == 0)
            {
                return NoContent();
            }
            return Ok(products);
        }
        [HttpGet("{id}/image")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProductImage(int id)
        {
            Nutrition product = parserManager.GetSingleProduct(id);
            if (product == null || string.IsNullOrEmpty(product.ImagePath))
                return NotFound();


            product.ImagePath = product.ImagePath.Replace('/', Path.DirectorySeparatorChar);
            if (!System.IO.File.Exists(product.ImagePath))
                return NotFound();

            byte[] imageBytes = System.IO.File.ReadAllBytes(product.ImagePath); // Read the image file into a byte array
            FileInfo fileInfo = new FileInfo(product.ImagePath);
            
            string contentType = $"image/{fileInfo.Extension}"; 

            return File(imageBytes, contentType); // Serve the image file with the correct content type
        }
        [HttpGet("setup")]
        public void imgPathSetup() // if there is no ImagePath. Call this once to setup image paths in the database.
        {
            parserManager.ImagePathSetup();
        }

    }

}
