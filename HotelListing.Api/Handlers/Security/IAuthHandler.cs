using HotelListing.API.Commos.Domains.Users;
using HotelListing.API.Handlers.Security.Data;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.API.Handlers.Security
{
    public interface IAuthHandler
    {
        Task<IEnumerable<IdentityError>> Register(ApiUserDto apiuser);

        Task<AuthResponseDto> Login(ApiUserLoginDto loginDto);
        
        Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request);
    }
}
