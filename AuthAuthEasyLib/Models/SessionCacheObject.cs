using AuthAuthEasyLib.Bases;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthAuthEasyLib.Models
{
    public class SessionCacheObject
    {
        public string UserId { get; set; }
        public Token Token { get; set; }
        public List<Role> Roles { get; set; } = new List<Role>();
        public TimeSpan UnusedExpiration { get; set; }
        public DateTime? AbsoluteExpiration { get; set; }

    }
}
