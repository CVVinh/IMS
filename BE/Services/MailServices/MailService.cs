using BE.Data.Dtos.MailDtos;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace BE.Services.MailServices
{
    public class MailService : IMailService
    {
        public async Task<bool> SendMail(MailDto mailDto)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse("imsvhec@gmail.com");
            email.To.Add(MailboxAddress.Parse(mailDto.To));
            email.Subject = mailDto.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = mailDto.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("imsvhec@gmail.com", "dpjldpazjawenmzo");
            try
            {
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
