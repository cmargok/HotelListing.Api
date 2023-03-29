using AutoMapper;
using HotelListing.API.Commos.Domains.Users;
using HotelListing.API.Handlers.Security.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.API.Handlers.Security
{
    public class AuthHandler : IAuthHandler
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private ApiUser _user;
        private const string _refreshToken = "RefreshToken";

        public AuthHandler(IMapper mapper, UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            _mapper = mapper;
            _userManager = userManager;
            _configuration = configuration;
        }   

        public async Task<AuthResponseDto> Login(ApiUserLoginDto loginDto)
        {           
             _user = await _userManager.FindByEmailAsync(loginDto.Email);

            if(_user is null) return null!;

            bool isValidCredentials = await _userManager.CheckPasswordAsync(_user, loginDto.Password);    
         
            if (!isValidCredentials) return null!;

            var token = await GenerateToken();

            return new AuthResponseDto
            {
                Token = token,
                UserId = _user.Id,
                RefreshToken = await CreateRefreshToken(),
            };

        }

        public async Task<IEnumerable<IdentityError>> Register(ApiUserDto apiuser)
        {
            var user = _mapper.Map<ApiUser>(apiuser);
            

            var result = await _userManager.CreateAsync(user,apiuser.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            return result.Errors;
        }

       

        private async Task<string> GenerateToken()
        {
            //var secret = Base64UrlEncoder.DecodeBytes(_configuration["JwtSettings:Key"]);
            var secret = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);
            var securityKey = new SymmetricSecurityKey(secret);

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(_user);

            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            var userClaims = await _userManager.GetClaimsAsync(_user);

            var claims = new List<Claim>()
            {
                new Claim("Version", "1.0"),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, _user.UserName),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Id", _user.Id),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Name, _user.FirstName+" "+_user.LastName),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, _user.Email),

            }.Union(userClaims).Union(roleClaims);


            var token = new JwtSecurityToken(
                issuer : _configuration["JwtSettings:Issuer"],
                audience : _configuration["JwtSettings:Audience"],
                claims : claims,
                expires : DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials : credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private async Task<string> CreateRefreshToken()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_user, _configuration["JwtSettings:Issuer"], _refreshToken);

            var newRefreshToken = await _userManager.GenerateUserTokenAsync(_user, _configuration["JwtSettings:Issuer"], _refreshToken);

            var result = await _userManager.SetAuthenticationTokenAsync(_user, _configuration["JwtSettings:Issuer"], _refreshToken, newRefreshToken);

            if (result.Succeeded) return newRefreshToken;
            throw new HttpRequestException("Token Cannot Be reGenerated");
        }

        public async Task<AuthResponseDto> VerifyRefreshToken(AuthResponseDto request)
        {
            var jwtSecutiryTokenHanlder = new JwtSecurityTokenHandler();

            var tokenContent = jwtSecutiryTokenHanlder.ReadJwtToken(request.Token);

            var username = tokenContent.Claims.ToList().FirstOrDefault(c => c.Type == System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)!.Value;

            _user = await _userManager.FindByNameAsync(username);

            if (_user is null || _user.Id != request.UserId) return null!;
            var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(_user, _configuration["JwtSettings:Issuer"], _refreshToken, request.RefreshToken);

            if (isValidRefreshToken)
            {
                var token = await GenerateToken();

                return new AuthResponseDto
                {
                    Token = token,
                    UserId = _user.Id,
                    RefreshToken = await CreateRefreshToken(),
                };
            }
            await _userManager.UpdateSecurityStampAsync(_user);
            return null!;
        }

    }
}
