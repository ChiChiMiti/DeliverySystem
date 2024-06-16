﻿using Microsoft.AspNetCore.Identity;

namespace DeliverySystem.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string PhoneNumber { get; set; }
    }
}
