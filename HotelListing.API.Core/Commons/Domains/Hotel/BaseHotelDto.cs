using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Commos.Domains.Hotel
{
    public class BaseHotelDto
    {
        [Required]
        public string Name { get; set; } = String.Empty;
        [Required]
        public string Address { get; set; } = String.Empty;
        public double? Rating { get; set; }

        [Required]
        [Range(1,int.MaxValue)]
        public int CountryId { get; set; }
    }
}
