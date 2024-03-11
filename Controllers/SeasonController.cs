using autox_data.Models;
using autox_data.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace autox_data.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SeasonController : ControllerBase
    {
        private readonly ILogger<SeasonController> _logger;
        private readonly WebScraper _scraper;

        public SeasonController(ILogger<SeasonController> logger)
        {
            _logger = logger;
            _scraper = new WebScraper();
        }

        [HttpGet]
        [Route("/allyears")]
        public async Task<ActionResult<string[]>> GetAllYears()
        {
            List<string> years = new List<string>();
            foreach(Season season in _scraper.seasons)
            {
                years.Add(season.Year);
            }
            Response.StatusCode = 200;
            Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:4200");
            return Ok(years);
        }

        [HttpGet("{year}")]
        public async Task<ActionResult<Season>> GetSeasonInfo(string year)
        {
            Season season = _scraper.GetSeason(year);

            Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:4200");

            if (season != null)
            {
                Response.StatusCode = 200;
                return season;
            }
            else
            {
                Response.StatusCode = 500;
                return NotFound();
            }
        }
    }
}
