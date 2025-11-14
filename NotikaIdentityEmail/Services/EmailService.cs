using MailKit.Net.Smtp;
using MimeKit;
using NotikaIdentityEmail.Entities;

namespace NotikaIdentityEmail.Services;

public interface IEmailService
{
    Task SendActivationMailAsync(AppUser user);
}

public class EmailService : IEmailService
{
    public async Task SendActivationMailAsync(AppUser appUser)
    {
        MimeMessage mimeMessage = new();
        mimeMessage.From.Add(new MailboxAddress("Notika Admin", "bs.10.erdn@gmail.com"));
        mimeMessage.To.Add(new MailboxAddress("User", appUser.Email));

        BodyBuilder bodyBuilder = new()
        {
            TextBody = $"Merhaba {appUser.FirstName} {appUser.LastName},\n\n" +
                       $"Aktivasyon Kodunuz: {appUser.ActivationCode}\n\n" +
                       "Lütfen bu kodu Email Aktivasyon alanına giriniz.\n\n" +
                       "İyi Dileklerimizle,\nNotika Team",
        };

        mimeMessage.Body = bodyBuilder.ToMessageBody();
        mimeMessage.Subject = "Hesap Aktivasyonu";

        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync("smtp.gmail.com", 587, false);
        await smtpClient.AuthenticateAsync("bs.10.erdn@gmail.com", "xrwi kbkk augj wdjl");
        await smtpClient.SendAsync(mimeMessage);
        await smtpClient.DisconnectAsync(true);
    }
}
