using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.API.Commos.Domains.Pagging;
using HotelListing.API.Data;
using HotelListing.API.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly HotelListingDBContext _context;
        private readonly IMapper _mapper;

        public GenericRepository(HotelListingDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<T> AddAsync(T entity)
        {
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<List<T>> GetAllAsync() 

            => await _context.Set<T>().ToListAsync();
        public async Task<T> GetAsync(int? id)
        {
            if (id is null) return null!;

            return (await _context.Set<T>().FindAsync(id))!;
        }
        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();

        }
        public async Task Deletesync(int id)
        {
            var entity = await GetAsync(id);
            _context.Set<T>().Remove(entity);

        }
        public async Task<bool> Exists(int id) 
        {
            var entity = await GetAsync(id);
            return entity != null;
        }

        public async Task<PagedResult<TResult>> GetAllPagedAsync<TResult>(QueryParameters queryParameters)
        {    

            var totalSize = await _context.Set<T>().CountAsync();


            decimal MaxPages = Math.Ceiling((decimal)totalSize / queryParameters.PageSize);


            if(MaxPages < queryParameters.PageNumber) 
            {
                return new PagedResult<TResult>
                {
                    Items = null!,
                    PageNumber = queryParameters.PageNumber,
                    RecordNumber = 0,
                    TotalCount = totalSize
                };
            }


            var items = await _context.Set<T>()
                .Skip(queryParameters.StartIndex)
                .Take(queryParameters.PageSize)
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();


            if (items.Count <= queryParameters.PageSize)
            {
                queryParameters.PageSize = items.Count;
            }

            return new PagedResult<TResult>
            {
                Items = items,
                PageNumber = queryParameters.PageNumber,
                RecordNumber = queryParameters.PageSize,
                TotalCount = totalSize
            };
        }
    }
}
