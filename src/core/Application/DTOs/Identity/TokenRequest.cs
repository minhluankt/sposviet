﻿namespace Application.DTOs.Identity
{
    public class TokenRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool isOwner { get; set; } = false;
    }
    public class RefreshTokenModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}