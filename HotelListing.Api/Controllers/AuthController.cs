using HotelListing.API.Commos.Domains.Users;
using HotelListing.API.Handlers.Security;
using HotelListing.API.Handlers.Security.Data;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthHandler _Auth;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthHandler Auth, ILogger<AuthController> logger)
        {
            _Auth = Auth;
            this._logger = logger;
        }

        // POST : api/Auth/Register

        /// <summary>
        /// Registrar un nuevo usuario
        /// </summary>
        /// <param name="apiUserDto"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]// retorna por defecto un code 400, por si hay error
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Register([FromBody]ApiUserDto apiUserDto)
        {
            _logger.LogInformation($"Registration Attempt for {apiUserDto.Email}");
            var errors = await _Auth.Register(apiUserDto);

            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            _logger.LogInformation($"Register Completed for {apiUserDto.Email}");
            return Ok();
        }

        // POST : api/Auth/Login
        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]// retorna por defecto un code 400, por si hay error
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] ApiUserLoginDto apiUserDto)
        {
            
            try
            {
                _logger.LogInformation($"Login Attempt for {apiUserDto.Email}");
                var AuthResponse = await _Auth.Login(apiUserDto);

                if (AuthResponse is null)
                {
                    return Unauthorized("Veo veo, quien es ud");
                }
                return Ok(AuthResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in the {nameof(Login)}"); 
                return Problem($"Something went wrong in the {nameof(Login)}", statusCode: 500);
            }
           
        }

        // POST : api/Auth/RefreshToken
        [HttpPost("RefreshToken")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]// retorna por defecto un code 400, por si hay error
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken([FromBody] AuthResponseDto apiUserDto)
        {
            var AuthResponse = await _Auth.VerifyRefreshToken(apiUserDto);

            if (AuthResponse is null)
            {
                return Unauthorized("Veo veo, quien es ud");
            }
            return Ok(AuthResponse);
        }

    }
}
