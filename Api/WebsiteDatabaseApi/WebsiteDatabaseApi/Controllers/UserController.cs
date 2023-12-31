using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace WebsiteDatabaseApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly DatabaseConnection _db;

        public UserController(ILogger<UserController> logger, DatabaseConnection db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _db.LoadUsers();
                if(users == null)
                {
                    return Ok("No users registered");
                }
                else
                {
                    return Ok(users);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUserById")]
        public IActionResult GetUserById(int userId)
        {
            try
            {
                if (_db.CheckIfUserExist(userId) == false)
                {
                    return BadRequest("User does not exist");
                }
                else
                {
                    var users = _db.LoadUsers();
                    var userDetails = users.Where(x => x.Id == userId).ToList();
                    return Ok(userDetails);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllSellers")]
        public IActionResult GetAllSellers(int sellerId)
        {
            try
            {
                var sellers = _db.GetSellers();
                if (sellers == null)
                {
                    return Ok("No sellers registered");
                }
                else
                {
                    return Ok(sellers);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetSellerById")]
        public IActionResult GetSellerById(int sellerId)
        {
            try
            {
                if (_db.CheckIfSellerExist(sellerId) == false)
                {
                    return BadRequest("Seller does not exist");
                }
                else
                {
                    var users = _db.LoadUsers();
                    var userDetails = users.Where(x => x.Id == sellerId).ToList();
                    return Ok(userDetails);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetWishlistForUser")]
        public IActionResult GetWishlistForUser(int userId)
        {
            try
            {
                if(_db.CheckIfUserExist(userId) == false)
                {
                    return BadRequest("User does not exist");
                }
                else
                {
                    var wishlist = _db.GetWishlistForUser(userId);
                    if (wishlist == null)
                    {
                        return Ok("User has no wishlist");
                    }
                    else
                    {
                        return Ok(wishlist);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCartForUser")]
        public IActionResult GetCartForUser(int userId)
        {
            try
            {
                if(_db.CheckIfUserExist(userId) == false)
                {
                    return BadRequest("User does not exist");
                }
                else
                {
                    var cart = _db.GetCartForUser(userId);
                    if (cart == null)
                    {
                        return Ok("User has no cart currently");
                    }
                    else
                    {
                        return Ok(cart);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser(string FirstName, string LastName, string Street, int StreetNumber, string City, int PostNumber, string Email, string password)
        {
            try
            {
                _db.CreateUser(FirstName, LastName, Street, StreetNumber, City, PostNumber, Email, password);
                return Ok("User created");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CreateSeller")]
        public IActionResult CreateSeller(string FullName, string email, string phone, string password)
        {
            try
            {
                _db.CreateSeller(FullName, email, phone, password);
                return Ok("User created");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddProductToWishlist")]
        public IActionResult AddProductToWishlist(int userId, int productId)
        {
            try
            {
                if(_db.CheckIfProductExist(productId) == false || _db.CheckIfUserExist(userId) == false)
                {
                    return BadRequest("User or Product does not exist");
                }
                _db.AddToWishlist(userId, productId);
                return Ok("Product added");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UserAddsToCart")]
        public IActionResult UserAddCart(int productId, int userId, string Size)
        {
            try
            {
                if (_db.CheckIfUserExist(userId) == false || _db.CheckIfProductExist(productId) == false)
                {
                    return BadRequest("User or Product does not exist");
                }

                string[] allowedSizes = {"Small", "Medium", "Large", "XL", "Size38", "Size39", "Size40", "Size41", "Size42", "Size43", "Size44", "Size45", "Size46"};

                bool isSizeAllowed = allowedSizes.Any(s => s == Size);

                if (isSizeAllowed == true)
                {
                    bool isInStock = _db.CheckIfProductAreInStock(productId, Size);

                    if(isInStock == true) 
                    {
                        _db.AddProductToCart(productId, userId, Size);
                        return Ok("Item added to cart");
                    }
                    else
                    {
                        return BadRequest("Product not in stock and therefore not added to cart");
                    }
                }
                else
                {
                    return BadRequest("Size not accepted");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UserBuysCart")]
        public IActionResult UserBuyCart(int userId)
        {
            try
            {
                if(_db.GetCartForUser(userId) == null)
                {
                    return BadRequest("User has no cart");
                }
                else
                {
                    _db.UserPaysCart(userId);
                    return Ok("User bought his cart");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateUserInformation")]
        public IActionResult UpdateInfoUser(int userId, string? FirstName, string? LastName, string? street, int? streetNum, int? postNum, string? email, string? password)
        {
            try
            {
                if(_db.CheckIfUserExist(userId) == false)
                {
                    return BadRequest("UserId does not exist");
                }
                string response = _db.UpdateUserInfo(userId, FirstName, LastName, street, streetNum, postNum, email, password);

                if(response == "No fields updated")
                {
                    return BadRequest("No fields updated. Could be because of a lack of information");
                }

                return Ok(response);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateSellerInformation")]
        public IActionResult UpdateInfoSeller(int sellerId, string? FullName, string? Email, string? Phone, string? Password)
        {
            try
            {
                if(_db.CheckIfSellerExist(sellerId) == false)
                {
                    return BadRequest("SellerId does not exist");
                }
                string response = _db.UpdateSellerInfo(sellerId, FullName, Email, Phone, Password);

                if(response == "No fields updated")
                {
                    return BadRequest("No fields updated. Could be because of a lack of information");
                }

                return Ok(response);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("CalcSellerReviewRating")]
        public IActionResult CalcRatingReview(int sellerId)
        {
            try
            {
                if (_db.CheckIfSellerExist(sellerId) == false)
                {
                    return BadRequest("SellerId does not exist");
                }
                double rating = _db.CalcReviewAverageRatingForSeller(sellerId);
                if (rating == -1)
                {
                    return BadRequest("Calc failed or there was no reviews.");
                }
                return Ok("Rating for seller is: " + rating);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("RemoveProductFromCart")]
        public IActionResult RemoveProductFromCart(int userId, int productId)
        {
            try
            {
                _db.RemoveProductFromCart(userId, productId);
                return Ok("Product removed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("RemoveProductFromWishlist")]
        public IActionResult RemoveProductFromWishlist(int userId, int productId)
        {
            try
            {
                _db.RemoveProductFromWishList(userId, productId);
                return Ok("Product Removed");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteUserId")]
        public IActionResult DeleteUser(int userId)
        {
            try
            {
                _db.DeleteUser(userId);
                return Ok("User deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("DeleteSellerId")]
        public IActionResult DeleteSeller(int sellerId)
        {
            try
            {
                _db.DeleteSeller(sellerId);
                return Ok("User deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}