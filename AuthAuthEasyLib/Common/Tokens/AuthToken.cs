using System;

namespace AuthAuthEasyLib.Common.Tokens
{
    [Serializable]
    public class AuthToken : Bases.Token
    {
        public AuthToken(string key, TimeSpan span)
        {
            Key = key;
            Expiration = DateTime.Now.Add(span);
            TokenCode = 1;
            Description = "Auth Token";
        }
       
    }
}
