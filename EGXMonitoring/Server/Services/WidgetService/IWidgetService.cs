using EGXMonitoring.Shared.DTOS;
using Newtonsoft.Json.Linq;
using System.Data;

namespace EGXMonitoring.Server.Services.WidgetService
{
    public interface IWidgetService
    {
        Task<ServiceResponse<List<ClientWidget>>> GetWidgetsInfo();
        List<Dictionary<string, object>> GetWidgetsData(Widget widgetsInfo);
    }
}
