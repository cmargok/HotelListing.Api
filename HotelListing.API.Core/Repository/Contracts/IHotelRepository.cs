using AutoMapper;
using HotelListing.API.Data;
using HotelListing.API.Data.Models;

namespace HotelListing.API.Repository.Contracts
{
    public interface IHotelRepository : IGenericRepository<Hotel>
    {
    }
    public class HotelRepository : GenericRepository<Hotel>, IHotelRepository
    {
        private readonly HotelListingDBContext _context;
        public HotelRepository(HotelListingDBContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
        }
    }
}
