using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Commos.Domains.Country
{
    public class BaseCountryDto
    {
        [Required]
        public string Name { get; set; }
        public string ShortName { get; set; }
    }


}
