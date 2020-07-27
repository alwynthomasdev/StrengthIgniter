using System;
using System.Net;
using System.Threading.Tasks;
using CodeFluff.Extensions.IEnumerable;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace StrengthIgniter.Core.Utils
{
    public interface IEmailUtility
    {
        /// <summary>
        /// Sends an email message
        /// </summary>
        /// <param name="Message">Email object with all email parameters</param>
        Task SendAsync(EmailMessageModel Message);

        /// <summary>
        /// Sends an email message
        /// </summary>
        /// <param name="Message">Email object with all email parameters</param>
        void Send(EmailMessageModel Message);
    }

    public class EmailUtility : IEmailUtility
    {
        private readonly EmailConfiguration _Config;
        private readonly ILogger _Logger;

        /// <summary>
        /// Email service ALL email sending is managed here
        /// </summary>
        /// <param name="Settings">Email settings such as SMTP and default parameters</param>
        public EmailUtility(
            EmailConfiguration config,
            ILoggerFactory loggerFactory
        )
        {
            //CTOR
            _Config = config;
            _Logger = loggerFactory.CreateLogger(typeof(EmailUtility));
        }

        public void Send(EmailMessageModel Message)
        {
            var mailMessage = BuildEmail(Message);

            try
            {
                using (var client = CreateClient())
                    client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw RaiseSendEmailException(ex, mailMessage);
            }

            _Logger.Log(LogLevel.Information, $"Email of subject '{Message.Subject}' was sent to recipients: {Message.To.ToCsvString()}");
        }

        public async Task SendAsync(EmailMessageModel Message)
        {
            var mailMessage = BuildEmail(Message);

            try
            {
                using (var client = await CreateClientAsync())
                    await client.SendAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw RaiseSendEmailException(ex, mailMessage);
            }

            _Logger.Log(LogLevel.Information, $"Email of subject '{Message.Subject}' was sent to recipients: {Message.To.ToCsvString()}");
        }

        #region Private Methods

        private async Task<SmtpClient> CreateClientAsync()
        {
            var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(_Config.SmtpHost, _Config.SmtpPort);
            await smtpClient.AuthenticateAsync(new NetworkCredential() { UserName = this._Config.SmtpUsername, Password = this._Config.SmtpPassword });

            return smtpClient;
        }
        private SmtpClient CreateClient()
        {
            var smtpClient = new SmtpClient();
            smtpClient.Connect(_Config.SmtpHost, _Config.SmtpPort);
            smtpClient.Authenticate(new NetworkCredential() { UserName = this._Config.SmtpUsername, Password = this._Config.SmtpPassword });

            return smtpClient;
        }

        private MimeMessage BuildEmail(EmailMessageModel msg)
        {
            msg.From = string.IsNullOrWhiteSpace(msg.From)
                ? _Config.DefaultFromAddress
                : msg.From;

            msg.FromName = string.IsNullOrWhiteSpace(msg.FromName)
                ? _Config.DefaultFromName
                : msg.FromName;

            var eMail = new MimeMessage();

            foreach (string email in msg.To)
                eMail.To.Add(new MailboxAddress(email));
            eMail.From.Add(new MailboxAddress(msg.FromName, msg.From));

            eMail.Subject = msg.From;
            eMail.Body = new TextPart(msg.IsHtml ? MimeKit.Text.TextFormat.Html : MimeKit.Text.TextFormat.Plain) { Text = msg.Body };

            return eMail;
        }

        private SendEmailException RaiseSendEmailException(Exception ex, MimeMessage mailMessage)
        {
            SendEmailException sendEmailException = new SendEmailException("Send Email Exception Raised.", ex);
            sendEmailException.Data.Add("EmailSettings", _Config);
            sendEmailException.Data.Add("MimeMessage", mailMessage);

            return sendEmailException;
        }

        #endregion

    }

    #region EmailUtility Models ...

    public class EmailConfiguration
    {
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string DefaultFromName { get; set; }
        public string DefaultFromAddress { get; set; }
    }

    /// <summary>
    /// Email Message Model
    /// </summary>
    public class EmailMessageModel
    {
        /// <summary>
        /// Recipients of email
        /// </summary>
        public string[] To { get; set; }

        /// <summary>
        /// Optional from address (default can be taken from settings, this overrides)
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Optional from name (default can be taken from settings, this overrides)
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// The subject of the email
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Email message body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Is the email in HTML format or plain text (be default this is set to true)
        /// </summary>
        public bool IsHtml { get; set; } = true;
    }

    #endregion ... EmailUtility Models

    public class SendEmailException : Exception
    {
        public SendEmailException()
        {
        }
        public SendEmailException(string message) : base(message)
        {
        }
        public SendEmailException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
