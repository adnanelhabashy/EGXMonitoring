using EGXMonitoring.Shared;
using static System.Net.WebRequestMethods;

namespace EGXMonitoring.Client.Services.MailService
{
    public class MailService : IMailService
    {
        private readonly HttpClient _http;
        public MailService(HttpClient http)
        {
            _http = http;
        }
        public async Task<ServiceResponse<Mail>> AddOrEdit(Mail mail)
        {
            var result = await _http.PostAsJsonAsync("api/Mail/addupdate", mail);
            var data = await result.Content.ReadFromJsonAsync<ServiceResponse<Mail>>();
    
            return data;
        }

        public async Task<ServiceResponse<Mail>> GetMail()
        {
            var mail = await _http.GetFromJsonAsync<ServiceResponse<Mail>>("api/Mail");
            return mail;
        }
    }
}
