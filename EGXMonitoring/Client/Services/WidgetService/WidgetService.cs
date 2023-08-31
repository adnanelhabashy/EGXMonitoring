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
        public async Task<ServiceResponse<List<ClientWidget>>> GetWidgetsInfo()
        {
            var widgets = await _http.GetFromJsonAsync<ServiceResponse<List<ClientWidget>>>("api/Widgets");
            return widgets;
        }

        public async Task<ServiceResponse<List<Dictionary<string, object>>>> GetWidgetData(ClientWidget widget)
        {
            var result = await _http.PostAsJsonAsync("api/Widgets/WidgetData", widget);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<List<Dictionary<string, object>>>>();
        }
    }
}
