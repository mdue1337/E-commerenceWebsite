using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace WebsiteDatabaseApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShopController : ControllerBase
    {
        private readonly ILogger<ShopController> _logger;
        private readonly DatabaseConnection _db;

        public ShopController(ILogger<ShopController> logger, DatabaseConnection db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpPost("CreateListing")]
        public async Task<IActionResult> CreateListing(string Name, int CategoryId, double Price, int Stock, string categoryColor, string categorySize, string categoryBrand, IFormFile picture)
        {
            if(CategoryId > 2 || CategoryId < 1)
            {
                return BadRequest("No CategoryId withing range");
            }

            if (picture == null || picture.Length == 0)
            {
                return BadRequest("No picture file was uploaded.");
            }

            using (var memoryStream = new MemoryStream())
            {
                await picture.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                using (var image = Image.Load(memoryStream))
                {
                    // image.Mutate(x => x.Resize(500, 500));
                    byte[] imageBytes;
                    using (var outputStream = new MemoryStream())
                    {
                        image.Save(outputStream, new JpegEncoder());
                        imageBytes = outputStream.ToArray();
                    }

                    string potientialErrorMessage = _db.CreateListing(Name, CategoryId, Price, Stock, imageBytes, categoryColor, categorySize, categoryBrand);

                    if (potientialErrorMessage == null) 
                    {
                        return Ok("Picture stored in database");
                    }
                    else
                    {
                        return BadRequest(potientialErrorMessage);
                    }
                }
            }
        }
    }
}