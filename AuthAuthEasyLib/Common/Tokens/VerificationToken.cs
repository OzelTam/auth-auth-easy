using System;

namespace AuthAuthEasyLib.Common.Tokens
{
    [Serializable]
    public class VerificationToken : Bases.Token
    {
        public VerificationToken(string key, TimeSpan span)
        {
            Key = key;
            Expiration = DateTime.Now.Add(span);
            TokenCode = 2;
            Description = "Verification Token";
        }
    }
}
