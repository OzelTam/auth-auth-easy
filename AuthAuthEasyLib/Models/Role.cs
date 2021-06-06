using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AuthAuthEasyLib.Models
{
    public class Role
    {
        public Role(string roleName, DateTime? expirationDate = null)
        {
            RoleName = roleName;
            ExpirationDate = expirationDate;
        }
        [Key]
        [BsonIgnore]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string RoleName { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
