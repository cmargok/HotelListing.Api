﻿using Microsoft.AspNetCore.Identity;
using System.Data.Common;

namespace HotelListing.API.Handlers.Security.Data
{
    public class ApiUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
