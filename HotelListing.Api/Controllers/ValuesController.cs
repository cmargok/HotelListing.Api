using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0", Deprecated = true)]
    public class ValuesController : ControllerBase
    {
        private static readonly string[] sum = new[]
        {
            "freezing","bracing","chilly","wild","freezing","warm","balmy","hot","Sweltering","Schoring",
        };
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(ILogger<ValuesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<jjota> Get()
        {
            
            return Enumerable.Range(1,5).Select(i => new jjota
            {
                Summary = sum[Random.Shared.Next(sum.Length)],
            }).ToArray();
        }
    }

    public class jjota
    {
        public string Summary { get; set; }
    }

    
}
