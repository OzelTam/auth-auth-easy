using AuthAuthEasyLib.Attributes;
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
        public string _Id { get; set; }

        [Unique]
        public string Password { get; set; }
        public bool IsVerified { get; set; }
        public List<Token> Tokens { get; set; } = new List<Token>();
        public List<string> Roles { get; set; } = new List<string> { "user" };
    }
}
