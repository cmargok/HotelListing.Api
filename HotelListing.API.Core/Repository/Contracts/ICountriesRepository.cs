using HotelListing.API.Data.Models;

namespace HotelListing.API.Repository.Contracts
{
    public interface ICountriesRepository : IGenericRepository<Country>
    {
        Task<Country> GetCountryDetails(int id);
    }
  
}
