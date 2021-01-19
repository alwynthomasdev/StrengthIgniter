using System;
using System.Collections.Generic;
using System.Text;

namespace StrengthIgniter.Service.Email
{
    public interface IEmailService
    {
        void SendEmail(SendEmailRequest request);
    }
}
