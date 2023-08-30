using EGXMonitoring.Server.Data;
using EGXMonitoring.Shared.DTOS;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace EGXMonitoring.Server.Services.WidgetService
{
    public class WidgetService:IWidgetService
    {
        private readonly DataContext _context;

        public WidgetService(DataContext context)
        {
            _context = context;
        }


        public async Task<List<ServiceResponse<Widget>>> GetWidgetsInfo()
        {
            var widgetsInfo = await _context.Widgets.ToListAsync();
            var result = new List<ServiceResponse<Widget>>();

            foreach (var widgetInfo in widgetsInfo)
            {
                result.Add(new ServiceResponse<Widget>()
                {
                    Data = widgetInfo
                });
            }

            foreach (var widgetInfo in result)
            {
                widgetInfo.Message = GetWidgetsData(widgetInfo.Data);
            }

            return result;
        }

        public string GetWidgetsData(Widget widgetInfo)
        {
            if (widgetInfo != null)
            {
                string connectionString = widgetInfo.CONNCETIONSTRINGID;

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand(widgetInfo.SQLCOMMAND, connection))
                        {
                            using (OracleDataReader reader = command.ExecuteReader())
                            {
                                DataTable dataTable = new DataTable();
                                dataTable.Load(reader);

                                string dataTableJson = JsonConvert.SerializeObject(dataTable);
                                return dataTableJson;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred while executing the query: " + ex.Message);
                        return null;
                    }
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
