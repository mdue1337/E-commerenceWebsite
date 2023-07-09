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
    }
}