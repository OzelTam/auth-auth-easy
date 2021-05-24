using System;

namespace AuthAuthEasyLib
{
    public class RegisterOptions
    {
        public TimeSpan VerificationSpan { get; set; }

        public bool RequireUniqueUsernames { get; set; }

        public bool RequireUniqueEmails { get; set; }

        public bool RequireUniquePhoneNumbers { get; set; }
    }
}
