using System;

namespace AuthAuthEasyLib.Tokens
{
    [Serializable]
    public class VerificationToken : Bases.Token
    {
        public VerificationToken(string key, TimeSpan? span)
        {
            Key = key;

            if(span == null)
                Expiration = null;
            else
                Expiration = DateTime.Now.Add(span.Value);


            TokenCode = 2;
            Description = "Verification Token";
        }
    }
}
