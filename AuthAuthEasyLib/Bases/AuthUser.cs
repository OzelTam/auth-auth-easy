using AuthAuthEasyLib.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;

namespace AuthAuthEasyLib.Bases
{
    [Serializable]
    public class AuthUser : IAuthUser
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string _Id { get ; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool IsVerified { get; set; }
        public string PhoneNumber { get; set; }
        public List<Token> Tokens { get; set; } = new List<Token>();
        public List<string> Roles { get; set; } = new List<string> { "User" };
    }
}
