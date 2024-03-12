using autox_data.Models;
using autox_data.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

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
            return Ok(years);
        }

        [HttpGet("{year}")]
        public async Task<ActionResult<Season>> GetSeasonInfo(string year)
        {
            if (year == "undefined")
                return BadRequest();

            Season season = _scraper.GetSeason(year);

            if (season != null)
                return Ok(season);
            return NotFound();

        }
    }
}
