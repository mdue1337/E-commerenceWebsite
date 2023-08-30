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
            try
            {
                var products = _db.GetAllProducts();

                if (products == null)
                {
                    return BadRequest("No products found");
                }
                else
                {
                    return Ok(products);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("GetAllProductsByCategory")]
        public IActionResult GetAllProductsByCategory(int category)
        {
            List<ProductsModel> products = _db.GetAllProducts();

            var product = products.FirstOrDefault(p => p.CategoryId == category);

            if (product == null)
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
            if (_db.CheckIfProductExist(productId) == true)
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

        [HttpGet("GetReviewsForProduct")]
        public IActionResult GetAllReviewsForProduct(int productId)
        {
            try
            {
                if(_db.CheckIfProductExist(productId) == true)
                {
                    var products = _db.GetAllProducts();
                    if(products == null)
                    {
                        return Ok("No reviews found for product");
                    }
                    else
                    {
                        return Ok(products);
                    }
                }
                else
                {
                    return BadRequest("ProductId does not exist");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateListingClothes")]
        public async Task<IActionResult> CreateListingClothes(int SellerId, string Name, double Price, string Color, string Brand, IFormFile picture, [FromForm] int[] sizes)
        {
            if (_db.CheckIfSellerExist(SellerId) == false)
            {
                return BadRequest("Seller does not exist");
            }

            if (sizes == null || sizes.Length == 0 || picture == null || picture.Length == 0)
            {
                return BadRequest("Picture or size array is not suffient");
            }

            if (sizes!.Length == 4)
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await picture.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        using (var image = Image.Load(memoryStream))
                        {
                            byte[] imageBytes;
                            using (var outputStream = new MemoryStream())
                            {
                                image.Save(outputStream, new JpegEncoder());
                                imageBytes = outputStream.ToArray();
                            }
                            string potientialErrorMessage = _db.CreateListingClothes(sizes, Color, Brand, Name, Price, imageBytes, SellerId);
                            if (potientialErrorMessage == null)
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

        [HttpPost("CreateListingShoes")]
        public async Task<IActionResult> CreateListingShoes(int SellerId, string Name, double Price, string Color, string Brand, IFormFile picture, [FromForm] int[] sizes)
        {
            if (_db.CheckIfSellerExist(SellerId) == false)
            {
                return BadRequest("Seller does not exist");
            }

            if (sizes == null || sizes.Length == 0 || picture == null || picture.Length == 0)
            {
               return BadRequest("Picture or size array is not suffient");
            }
            
            if (sizes!.Length == 9)
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
                            string potientialErrorMessage = _db.CreateListingShoes(sizes, Color, Brand, Name, Price, imageBytes, SellerId);
                            if (potientialErrorMessage == null)
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
                return BadRequest("size array does not contain excatly 9 elements");
            }
        }
        [HttpPost("CreateReview")]
        public IActionResult CreateReview(int productId, int userId, int rating, [FromQuery] string? text = null)
        {
            string timestamp = DateTime.UtcNow.ToString();

            if (_db.CheckIfProductExist(productId) != true)
            {
                return BadRequest("ProductId does not exist");
            }
            if (_db.CheckIfUserExist(userId) != true)
            {
                return BadRequest("UserId does not exist");
            }
            if (rating > 5 || rating < 0)
            {
                return BadRequest("Rating too high or too low");
            }

            _db.CreateReview(productId, userId, rating, text, timestamp);

            return Ok("Created with timestamp: " + timestamp);
        }

        [HttpPut("UpdateClothingStock")]
        public IActionResult UpdateClothingStock(int productId, [FromForm] int?[] clothingsizes)
        {
            int categoryId = 1;
            if (_db.CheckIfProductCategory(categoryId, productId) == false && _db.CheckIfProductExist(productId) == false)
            {
                return BadRequest("Product not found");
            }

            if (clothingsizes.Length != 4)
            {
                return BadRequest("Please send 4 elements in the size int array");
            }

            _db.UpdateClothingStock(productId, clothingsizes);

            return Ok("Stock updated");
        }

        [HttpPut("UpdateShoesStock")]
        public IActionResult UpdateShoesStock(int productId, [FromForm] int?[] Shoesizes)
        {
            int categoryId = 2;
            if (_db.CheckIfProductCategory(categoryId, productId) == false && _db.CheckIfProductExist(productId) == false)
            {
                return BadRequest("Product not found");
            }

            if(Shoesizes.Length != 9)
            {
                return BadRequest("Please send 9 elements in the size int array");
            }

            _db.UpdateShoesStock(productId, Shoesizes);

            return Ok("Stock updated");
        }

        [HttpPut("UpdateProductPrice")]
        public IActionResult UpdatePrice(int productId, double price)
        {
            if(_db.CheckIfProductExist(productId) == false)
            {
                return BadRequest("ProductId does not exist");
            }
            _db.UpdateProductPrize(productId, price);

            return Ok("Price updated");
        }

        [HttpDelete("DeleteReview")]
        public IActionResult DeleteReview(int userId, int reviewId)
        {
            try
            {
                if(_db.CheckIfReviewExist(reviewId) == true && _db.CheckIfUserExist(userId))
                {
                    _db.DeleteReview(userId, reviewId);
                    return Ok("Review deleted");
                }
                else
                {
                    return BadRequest("ReviewId does not exist or UserId does not exist");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteProduct")]
        public IActionResult DeleteProduct(int productId)
        {
            try
            {
                if(_db.CheckIfProductExist(productId) == false)
                {
                    return BadRequest("ProductId does not exist");
                }
                else
                {
                    _db.DeleteProduct(productId);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}