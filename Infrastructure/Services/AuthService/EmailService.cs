using System.Net;
using System.Net.Mail;
using Application.Interfaces.Services.AuthService;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.AuthService;

public class EmailService(IConfiguration configuration) : IEmailService
{
    public async Task SendAsync(string to, string subject, string body)
    {
        var host = configuration["Smtp:Host"]!;
        var port = int.Parse(configuration["Smtp:Port"]!);
        var username = configuration["Smtp:Username"]!;
        var password = configuration["Smtp:Password"]!;
        var from = configuration["Smtp:From"]!;

        using var client = new SmtpClient(host, port);
        client.Credentials = new NetworkCredential(username, password);
        client.EnableSsl = true;

        var mail = new MailMessage
        {
            From = new MailAddress(from, "Online Courses"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(to);
        await client.SendMailAsync(mail);
    }
}