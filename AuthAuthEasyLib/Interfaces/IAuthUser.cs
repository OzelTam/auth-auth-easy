using AuthAuthEasyLib.Bases;
using AuthAuthEasyLib.Models;
using System;
using System.Collections.Generic;

namespace AuthAuthEasyLib.Interfaces
{
    public interface IAuthUser
    {
        string _Id { get; set; }
        string Password { get; set; }
        bool IsVerified { get; set; }
        List<Token> Tokens { get; set; }
        List<Role> Roles { get; set; }

    }
}
