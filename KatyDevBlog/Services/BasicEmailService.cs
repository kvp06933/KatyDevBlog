using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KatyDevBlog.Services
{
    public class BasicEmailService : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public BasicEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var newEmail = new MimeMessage();

            //I need to talk to appsettings.json
            var emailAddress = _configuration["SmtpSettings:Email"];
            newEmail.Sender = MailboxAddress.Parse(emailAddress);
            newEmail.To.Add(MailboxAddress.Parse(emailAddress));
            newEmail.Subject = subject;

            var body = new BodyBuilder();
            body.HtmlBody = htmlMessage;
            newEmail.Body = body.ToMessageBody();

            //Configure the SMTP server to send the newEmail
            using SmtpClient smtpClient = new();
            var host = _configuration["SmtpSettings:Host"];
            var port = Convert.ToInt32(_configuration["SmtpSettings:Port"]);
            smtpClient.Connect(host, port, MailKit.Security.SecureSocketOptions.StartTls);
            smtpClient.Authenticate(emailAddress, _configuration["SmtpSettings:Password"]);
            await smtpClient.SendAsync(newEmail);

            smtpClient.Disconnect(true);
        }
    }
}