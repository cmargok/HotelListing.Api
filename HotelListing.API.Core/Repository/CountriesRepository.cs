using AutoMapper;
using HotelListing.API.Data;
using HotelListing.API.Data.Models;
using HotelListing.API.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository
{
    public class CountriesRepository : GenericRepository<Country>, ICountriesRepository
    {
        private readonly HotelListingDBContext _context;
        public CountriesRepository(HotelListingDBContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }

        public async Task<Country> GetCountryDetails(int id)
        {
            var country = await _context.Countries
                                .Include(ct => ct.Hotels)
                                .SingleOrDefaultAsync(ct => ct.Id == id);
            return country!;
        }
    }
}
