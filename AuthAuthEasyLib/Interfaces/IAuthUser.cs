using AuthAuthEasyLib.Bases;
using System;
using System.Collections.Generic;

namespace AuthAuthEasyLib.Interfaces
{
    public interface IAuthUser
    {
        string _Id { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string Email { get; set; }
        bool IsVerified { get; set; }
        string PhoneNumber { get; set; }
        List<Token> Tokens { get; set; }
        List<String> Roles { get; set; }

    }
}
