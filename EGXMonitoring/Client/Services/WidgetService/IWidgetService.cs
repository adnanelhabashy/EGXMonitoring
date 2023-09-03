using EGXMonitoring.Shared.DTOS;
using System.Data;

namespace EGXMonitoring.Client.Services.WidgetService
{
    public interface IWidgetService
    {
        Task<ServiceResponse<List<ClientWidget>>> GetWidgetsInfo();

        Task<ServiceResponse<DataTable>> GetWidgetData(ClientWidget widget);

    }
}
