using autox_data.Models;
using autox_data.Utils;
using Microsoft.AspNetCore.Mvc;

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
            if (name == "undefined" || year == "undefined")
                return BadRequest("Name or year parameter missing");

            Competitor comp = _scraper.GetCompetitor(name, year);

            if (comp != null)
                return Ok(comp);
            return NotFound();
        }
    }
}
