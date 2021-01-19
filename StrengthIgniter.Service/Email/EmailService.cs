using System;
using System.Net;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using StrengthIgniter.Dal.AuditEvent;
using StrengthIgniter.Dal.Common;
using StrengthIgniter.EmailTemplates;
using StrengthIgniter.Service.Common;
using StrengthIgniter.Models.AuditEvent;
using System.Collections.Generic;

namespace StrengthIgniter.Service.Email
{
    public class EmailService : DataServiceBase, IEmailService
    {
        private readonly EmailServiceConfiguration _Config;
        private readonly IEmailTemplateProvider _EmailTemplateProvider;

        public EmailService(
            EmailServiceConfiguration config,
            IEmailTemplateProvider emailTemplateProvider,
            //
            IDataAccessTransactionProvider transactionProvider,
            IAuditEventDataAccess auditEventDataAccess,
            //
            ILogger logger
        )
            :base(transactionProvider, auditEventDataAccess, logger)
        {
            _Config = config;
        }

        public void SendEmail(SendEmailRequest request)
        {
            try
            {
                request.ValidateAndThrow();
                MimeMessage message = CreateMimeMessage(request);

                using (SmtpClient client = CreateClient())
                {
                    client.Send(message);
                }
                LogInfo($"Email of subject '{request.Subject}' was sent to recipients: {request.ToAddress}");
                CreateEmailAuditEvent(message, request.UserReference, request.RelatedAuditEventId);
            }
            catch (Exception ex)
            {
                ThrowServiceException(ex, "SendEmail", request);
            }
        }

        //

        private SmtpClient CreateClient()
        {
            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Connect(_Config.SmtpHost, _Config.SmtpPort);
            smtpClient.Authenticate(new NetworkCredential() { UserName = this._Config.SmtpUsername, Password = this._Config.SmtpPassword });

            return smtpClient;
        }

        private MimeMessage CreateMimeMessage(SendEmailRequest request)
        {
            MimeMessage message = new MimeMessage();

            //TODO: email validation ???
            message.To.Add(MailboxAddress.Parse(request.ToAddress));

            string fromName = request.FromName ?? _Config.DefaultFromName;
            string fromAddress =  request.FromAddress ?? _Config.DefaultFromAddress;

            message.From.Add(new MailboxAddress(fromName, fromAddress));

            message.Subject = request.Subject;

            if(request.Template != null)
            {
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html) 
                { 
                    Text = _EmailTemplateProvider.Generate(request.Template) 
                };
            }
            else
            {
                message.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = request.Body };
            }

            return message;
        }

        private int CreateEmailAuditEvent(MimeMessage message, Guid? userReference, int? relatedAuditEventId = null)
        {
            IEnumerable<AuditEventItemModel> auditItems = new AuditEventItemModel[]
            {
                new AuditEventItemModel { Key = "SendTo", Value = message.To.ToString() },
                new AuditEventItemModel { Key = "SentFrom", Value = message.From.ToString() },
                new AuditEventItemModel { Key = "Subject", Value = message.Subject },
                new AuditEventItemModel { Key = "MessageBody", Value = message.Body.ToString() },
            };
            return CreateAuditEvent(AuditEventTypeConstants.EmailSent, "Subject: " + message.Subject, auditItems, userReference, relatedAuditEventId);
        }

        private void LogMessagesSend(SendEmailRequest request)
        {
        }

    }
}
