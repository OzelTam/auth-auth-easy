using AuthAuthEasyLib.Bases;
using System;

namespace AuthAuthEasyLib.Tokens
{
    public class ResetPasswordToken : Token
    {
        public ResetPasswordToken(string key, TimeSpan? span = null)
        {
            Key = key;
            if (span == null)
                Expiration = null;
            else
                Expiration = DateTime.Now.Add(span.Value);
            TokenCode = 3;
            Description = "Reset Password Token";
        }
    }
}
