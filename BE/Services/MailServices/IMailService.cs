using BE.Data.Dtos.MailDtos;

namespace BE.Services.MailServices
{
    public interface IMailService
    {
        Task<bool> SendMail(MailDto mailDto);
    }
}
