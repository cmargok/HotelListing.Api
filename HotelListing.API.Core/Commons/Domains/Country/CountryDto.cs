using HotelListing.API.Commos.Domains.Hotel;

namespace HotelListing.API.Commos.Domains.Country
{
    public class CountryDto : BaseCountryDto
    {
        public int Id { get; set; }
        public List<HotelDto> Hotels { get; set; }
    }


}
