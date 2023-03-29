using AutoMapper;
using HotelListing.API.Commos.Domains.Country;
using HotelListing.API.Commos.Domains.Hotel;
using HotelListing.API.Commos.Domains.Users;
using HotelListing.API.Data.Models;
using HotelListing.API.Handlers.Security.Data;

namespace HotelListing.API.Commos.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Country, CreateCountryDto>().ReverseMap();
            CreateMap<Country, GetCountryDto>().ReverseMap();
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<Country, UpdateCountryDto>().ReverseMap();


            CreateMap<Hotel, HotelDto>().ReverseMap();
            CreateMap<Hotel, CreateHotelDto>().ReverseMap();

            CreateMap<ApiUser, ApiUserDto>().ReverseMap();

        }
    }
}
