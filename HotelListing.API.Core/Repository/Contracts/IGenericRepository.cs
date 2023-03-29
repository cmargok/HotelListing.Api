using HotelListing.API.Commos.Domains.Pagging;

namespace HotelListing.API.Repository.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetAsync(int? id);
        Task<List<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task Deletesync(int id);
        Task<bool> Exists(int id);
        Task<PagedResult<TResult>> GetAllPagedAsync<TResult>(QueryParameters queryParameters);
    }
}
