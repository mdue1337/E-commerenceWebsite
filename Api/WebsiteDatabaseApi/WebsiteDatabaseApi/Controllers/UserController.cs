using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
                return Ok(_db.LoadUsers());
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

        [HttpPost("CalcSellerReviewRating")]
        public IActionResult CalcRatingReview(int sellerId)
        {
            try
            {
                if(_db.CheckIfSellerExist(sellerId) == false)
                {
                    return BadRequest("SellerId does not exist");
                }
                double rating = _db.CalcReviewAverageRatingForSeller(sellerId);
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