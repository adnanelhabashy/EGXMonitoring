using EGXMonitoring.Server.Data;
using EGXMonitoring.Shared.DTOS;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text.Json.Nodes;

namespace EGXMonitoring.Server.Services.WidgetService
{
    public class WidgetService : IWidgetService
    {
        private readonly DataContext _context;

        public WidgetService(DataContext context)
        {
            _context = context;
        }


        public async Task<ServiceResponse<List<ClientWidget>>> GetWidgetsInfo()
        {
            var widgetsInfo = await _context.Widgets.ToListAsync();
            var result = new ServiceResponse<List<ClientWidget>>();
            result.Data = new List<ClientWidget>();
            foreach (var widgetInfo in widgetsInfo)
            {
                result.Data.Add(new ClientWidget()
                {
                    WidgetInfo = widgetInfo,
                });
            }

            return result;
        }

        public ServiceResponse<List<Dictionary<string, object>>> GetWidgetData(Widget widgetInfo)
        {
            if (widgetInfo != null)
            {
                string connectionString = widgetInfo.CONNCETIONSTRINGID;

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand("Select * from DATAMONITOR", connection))
                        {
                            using (OracleDataReader reader = command.ExecuteReader())
                            {

                                DataTable dataTable = new DataTable();
                                dataTable.Load(reader);
                                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                                foreach (DataRow dataRow in dataTable.Rows)
                                {
                                    Dictionary<string, object> row = new Dictionary<string, object>();

                                    // Iterate over the columns in each row
                                    foreach (DataColumn column in dataTable.Columns)
                                    {
                                        // Get the column name and value for the current row
                                        string columnName = column.ColumnName;
                                        object columnValue = dataRow[columnName];

                                        // Add the column name and value to the dictionary
                                        row[columnName] = columnValue;
                                    }

                                    // Add the row to the list
                                    rows.Add(row);
                                }
                                return new ServiceResponse<List<Dictionary<string, object>>>()
                                {
                                    Data = rows
                                };    
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
                return null;
            }
        }

        public ServiceResponse<List<Dictionary<string, object>>> GetWidgetsData(Widget widgetsInfo)
        {
            throw new NotImplementedException();
        }
    }
}
