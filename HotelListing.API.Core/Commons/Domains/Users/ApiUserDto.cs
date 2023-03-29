using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Commos.Domains.Users
{
    public class ApiUserDto : ApiUserLoginDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        public string UserName { get; set; }

    }
    public class ApiUserLoginDto
    {        

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Your password is limited to {2} to {1} characters", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
