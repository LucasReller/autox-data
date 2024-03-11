using autox_data.Models;
using autox_data.Utils;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web.Http.Description;

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
            Driver driver = _scraper.GetDriver(name);
            Response.StatusCode = 200;
            Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
            return Ok(driver);
        }
    }
}
