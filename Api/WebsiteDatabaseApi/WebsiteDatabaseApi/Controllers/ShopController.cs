using Microsoft.AspNetCore.Mvc;

namespace WebsiteDatabaseApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShopController : ControllerBase
    {
        private readonly ILogger<ShopController> _logger;

        public ShopController(ILogger<ShopController> logger)
        {
            _logger = logger;
        }
    }
}