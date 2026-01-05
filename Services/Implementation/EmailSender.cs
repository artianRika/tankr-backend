using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

public class EmailSender
{
    private readonly IConfiguration _config;
    public EmailSender(IConfiguration config) => _config = config;

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        var msg = new MimeMessage();
        msg.From.Add(MailboxAddress.Parse(_config["Email:Smtp:From"]));
        msg.To.Add(MailboxAddress.Parse(toEmail));
        msg.Subject = subject;

        msg.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var smtp = new SmtpClient();
        smtp.CheckCertificateRevocation = false;

        await smtp.ConnectAsync(
            _config["Email:Smtp:Host"],
            int.Parse(_config["Email:Smtp:Port"]!),
            SecureSocketOptions.StartTls);

        await smtp.AuthenticateAsync(
            _config["Email:Smtp:User"],
            _config["Email:Smtp:Pass"]);

        await smtp.SendAsync(msg);
        await smtp.DisconnectAsync(true);
    }
}