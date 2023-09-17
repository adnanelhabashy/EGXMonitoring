using EGXMonitoring.Shared.DTOS;
using System.Data;

namespace EGXMonitoring.Client.Services.WidgetService
{
    public interface IWidgetService
    {
        event Action OnChange;
        Task<ServiceResponse<List<ClientWidget>>> GetWidgetsInfo();
        Task<ServiceResponse<DataTable>> GetWidgetData(ClientWidget widget);
        Task<ServiceResponse<ClientWidget>> AddWidget(ClientWidget widget);
        Task<ServiceResponse<ClientWidget>> UpdateWidget(ClientWidget widget);
        Task<ServiceResponse<ClientWidget>> DeleteWidget(ClientWidget widget);
        Task<ServiceResponse<List<TabLayouts>>> GetTabsLayout();
        Task<ServiceResponse<List<TabLayouts>>> SetTabsLayout(List<TabLayouts> TabsLayouts);
    }
}
