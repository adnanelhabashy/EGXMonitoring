using EGXMonitoring.Shared.DTOS;

namespace EGXMonitoring.Server.Services.WidgetService
{
    public interface IWidgetService
    {
        Task<List<ServiceResponse<Widget>>> GetWidgetsInfo();
        string GetWidgetsData(Widget widgetsInfo);
    }
}
