using System;

namespace AuthAuthEasyLib.Tokens
{
    [Serializable]
    public class AuthToken : Bases.Token
    {
        public AuthToken(string key, TimeSpan? span = null)
        {
            Key = key;
            if (span == null)
                Expiration = null;
            else
                Expiration = DateTime.Now.Add(span.Value);
            TokenCode = 1;
            Description = "Auth Token";
        }

    }
}
