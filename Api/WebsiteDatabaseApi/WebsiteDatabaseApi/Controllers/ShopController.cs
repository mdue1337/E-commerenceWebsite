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

        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            return Ok(_db.GetAllProducts());
        }

        [HttpGet("GetAllProductsByCategory")]
        public IActionResult GetAllProductsByCategory(int category)
        {
            List<ProductsModel> products = _db.GetAllProducts();
            bool foundMatch = false;

            foreach (var product in products)
            {
                if (product.CategoryId == category)
                {
                    foundMatch = true;
                }
            }

            if (!foundMatch)
            {
                return BadRequest("CategoryId not found");
            }
            else
            {
                var productsByCategory = products.Where(x => x.CategoryId == category).ToList();
                return Ok(productsByCategory);
            }
        }

        [HttpGet("GetProductById")]
        public IActionResult GetProductById(int productId)
        {
            // Does product exist?
            if(_db.CheckIfProductExist(productId) == true)
            {
                var products = _db.GetAllProducts();
                var product = products.Where(x => x.Id == productId).ToList();
                return Ok(product);
            }
            else
            {
                return BadRequest("Product does not exist");
            }
        }

        [HttpPost("CreateListingClothes")]
        public async Task<IActionResult> ListingClothes(string Name, double Price, string Color, string Brand, IFormFile picture, [FromForm] int[] sizes)
        {
            if (sizes == null || sizes.Length == 0 || picture == null || picture.Length == 0)
            {
                BadRequest("Picture or size array is not suffient");
            }
            if (sizes.Length == 4)
            {
                try
                {
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
                            string potientialErrorMessage = _db.CreateListingClothes(sizes, Color, Brand, Name, Price, imageBytes);
                            if(potientialErrorMessage == null)
                            {
                                return Ok("Listing created");
                            }
                            else
                            {
                                return BadRequest();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.ToString());
                }
            }
            else
            {
                return BadRequest("size array does not contain excatly 4 elements");
            }
        }
    }

    /*[HttpPost("CreateListingShoes")]
    public async Task<IActionResult> ListingClothes(string Name, double Price, string Color, string Brand, IFormFile picture, [FromForm] int[] sizes)
    {
        int CategoryId = 1;

        // Create ShoesPropoerties table

        // Create ShoesSizes table

        // Create listing

        return Ok()
    }*/
}