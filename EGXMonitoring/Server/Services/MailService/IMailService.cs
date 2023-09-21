using EGXMonitoring.Shared.DTOS;

namespace EGXMonitoring.Server.Services.MailService
{
    public interface IMailService
    {
        Task<ServiceResponse<Mail>> AddOrEdit(Mail mail);
        Task<ServiceResponse<Mail>> GetMail();
    }
}
