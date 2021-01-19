using StrengthIgniter.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Authentication
{
    public class AuthenticateResponse
    {
        public AuthenticateResponseTypeEnum ResponseType { get; internal set; }
        public Guid UserReference { get; internal set; }
        public string Username { get; internal set; }
        public UserTypeEnum UserType { get; internal set; }
    }

    

}
