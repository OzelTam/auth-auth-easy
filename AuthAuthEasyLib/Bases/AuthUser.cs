using AuthAuthEasyLib.Attributes;
using AuthAuthEasyLib.Interfaces;
using AuthAuthEasyLib.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthAuthEasyLib.Bases
{
    [Serializable]
    public class AuthUser : IAuthUser
    {
        [Key]
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string _Id { get; set; } = Guid.NewGuid().ToString();
        public string Password { get; set; }
        public bool IsVerified { get; set; }
        public List<Token> Tokens { get; set; } = new List<Token>();
        public List<Role> Roles { get; set; } = new List<Role>() { new Role("user",null)};
    }
}
