using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers.V2
{
    [Route("api/v{version:apiVersion}/Values")]
    [ApiController]
    [ApiVersion("2.0")]
    public class ValuesV2Controller : ControllerBase
    {
        private static readonly string[] sum = new[]
        {
            "freezing","bracing","chilly","wild","freezing","warm","balmy","hot","Sweltering","Schoring",
        };
        private readonly ILogger<ValuesV2Controller> _logger;

        public ValuesV2Controller(ILogger<ValuesV2Controller> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<jjota> Gets()
        {


            return sum.Select(x => new jjota { Summary = x }).AsEnumerable();
        }
    }
}
