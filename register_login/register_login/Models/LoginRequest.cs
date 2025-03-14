﻿using Microsoft.AspNetCore.Identity;

namespace register_login.Models
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
