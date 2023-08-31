using EGXMonitoring.Shared.DTOS;
using Newtonsoft.Json.Linq;
using System.Data;

namespace EGXMonitoring.Server.Services.WidgetService
{
    public interface IWidgetService
    {
        Task<ServiceResponse<List<ClientWidget>>> GetWidgetsInfo();
        ServiceResponse<List<Dictionary<string, object>>> GetWidgetData(Widget widgetInfo);
    }
}
