using EGXMonitoring.Shared.DTOS;
using Newtonsoft.Json.Linq;
using System.Data;

namespace EGXMonitoring.Server.Services.WidgetService
{
    public interface IWidgetService
    {
        Task<ServiceResponse<List<ClientWidget>>> GetWidgetsInfo();

        ServiceResponse<List<Dictionary<string, object>>> GetWidgetData(ClientWidget widgetInfo);
        ServiceResponse<List<Dictionary<string, object>>> GetStatusWidgetData(ClientWidget widgetInfo);
        Task<ServiceResponse<ClientWidget>> UpdateWidget(ClientWidget widget);
        Task<ServiceResponse<ClientWidget>> AddWidget(ClientWidget widget);
        Task<ServiceResponse<ClientWidget>> RemoveWidget(ClientWidget widget);
        Task<ServiceResponse<List<TabLayouts>>> GetTabLayouts();
        Task<ServiceResponse<List<TabLayouts>>> SetTabLayouts(List<TabLayouts> Layouts);
        Task<ServiceResponse<List<TabLayouts>>> DeleteTabLayouts(List<TabLayouts> Layouts);

    }
}
