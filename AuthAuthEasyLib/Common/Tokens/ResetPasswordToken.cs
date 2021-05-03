using AuthAuthEasyLib.Bases;
using System;

namespace AuthAuthEasyLib.Common.Tokens
{
    public class ResetPasswordToken:Token
    {
        public ResetPasswordToken(string key, TimeSpan span)
        {
            Key = key;
            Expiration = DateTime.Now.Add(span);
            TokenCode = 3;
            Description = "Reset Password Token";
        }
    }
}
