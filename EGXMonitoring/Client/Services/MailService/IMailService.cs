namespace EGXMonitoring.Client.Services.MailService
{
    public interface IMailService
    {
        Task<ServiceResponse<Mail>> AddOrEdit(Mail mail);
        Task<ServiceResponse<Mail>> GetMail();
    }
}
