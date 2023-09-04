using EGXMonitoring.Shared.DTOS;
using System.Collections.Generic;
using System.Data;
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

        public async Task<ServiceResponse<DataTable>> GetWidgetData(ClientWidget widget)
        {
            var result = await _http.PostAsJsonAsync("api/Widgets/WidgetData", widget);
            var data =  await result.Content.ReadFromJsonAsync<ServiceResponse<List<Dictionary<string, object>>>>();

            return new ServiceResponse<DataTable>() { 
            Data = ManipulateData(data.Data),
            Message = data.Message
            };

        }

        private DataTable ManipulateData(List<Dictionary<string, object>> dataList)
        {
            DataTable result = new DataTable();
            foreach (var key in dataList[0].Keys)
            {
                result.Columns.Add(key);
            }

            // Add rows to DataTable
            foreach (var dict in dataList)
            {
                DataRow row = result.NewRow();

                foreach (var pair in dict)
                {
                    row[pair.Key] = pair.Value.ToString();
                }

                result.Rows.Add(row);
            }

            return result;
        }

    
    }
}
