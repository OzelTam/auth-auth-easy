
using AuthAuthEasyLib.Tokens;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace AuthAuthEasyLib.Bases
{
    // Token Codes:
    //  1 - Auth Token
    //  2 - Verification Token
    //  3 - 


    [Serializable]
    [BsonKnownTypes(typeof(AuthToken), typeof(VerificationToken), typeof(ResetPasswordToken))]
    public class Token
    {
        [Key]
        public string Key { get; set; }
        public DateTime? Expiration { get; set; }
        /// <summary>
        /// Known <see cref="Token"/> Codes: <br/> <list type="bullet">
        /// <item> 1 = <see cref="AuthToken"/></item>
        /// <item> 2 = <see cref="VerificationToken"/></item>
        /// <item> 3 = <see cref="AuthToken"/></item>
        /// </list>
        /// </summary>
        public int TokenCode { get; set; }
        public string Description { get; set; }
    }
}
