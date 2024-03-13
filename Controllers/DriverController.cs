using autox_data.Models;
using autox_data.Utils;
using Microsoft.AspNetCore.Mvc;

namespace autox_data.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly ILogger<DriverController> _logger;
        private readonly WebScraper _scraper;

        public DriverController(ILogger<DriverController> logger)
        {
            _logger = logger;
            _scraper = new WebScraper();
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<ActionResult<Driver>> GetByName(string name)
        {
            if (name == "undefined")
                return BadRequest("No name parameter provided");

            Driver driver = _scraper.GetDriver(name);

            if (driver != null)
                return Ok(driver);
            return NotFound();
        }
    }
}
