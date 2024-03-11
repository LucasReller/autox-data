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
    public class CompetitorController : ControllerBase
    {
        private readonly ILogger<CompetitorController> _logger;
        private readonly WebScraper _scraper;

        public CompetitorController(ILogger<CompetitorController> logger)
        {
            _logger = logger;
            _scraper = new WebScraper();
        }
        [HttpGet]
        [Route("{name}")]
        public async Task<ActionResult<Competitor>> GetByName(string name, string year)
        {
            Competitor comp = _scraper.GetCompetitor(name, year);
            Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
            return Ok(comp);
        }
    }
}
