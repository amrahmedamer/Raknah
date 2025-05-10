using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using Raknah.Setting;

namespace Raknah.Services;

public class EmailService(IOptions<EmailSetting> emailSetting) : IEmailSender
{
    private readonly EmailSetting _emailSetting = emailSetting.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage()
        {
            Sender = MailboxAddress.Parse(_emailSetting.Mail)!,
            Subject = subject
        };
        message.To.Add(MailboxAddress.Parse(email));
        var body = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };
        message.Body = body.ToMessageBody();

        try
        {
            var clinet = new SmtpClient();

            clinet.Connect(_emailSetting.Host, _emailSetting.Port, SecureSocketOptions.StartTls);
            clinet.Authenticate(_emailSetting.Mail, _emailSetting.Password);
            await clinet.SendAsync(message);
            clinet.Disconnect(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }

}
