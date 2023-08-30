using EGXMonitoring.Shared.DTOS;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace EGXMonitoring.Client.Services.WidgetService
{
    public class WidgetService :IWidgetService
    {
        private readonly HttpClient _http;
        public WidgetService(HttpClient http)
        {
            _http = http;
        }
        public async Task<List<ServiceResponse<Widget>>> GetWidgets()
        {
            var widgets = await _http.GetFromJsonAsync<List<ServiceResponse<Widget>>>("api/Widgets");
            return widgets;
        }
    }
}
