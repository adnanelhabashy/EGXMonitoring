using EGXMonitoring.Server.Data;
using EGXMonitoring.Shared.DTOS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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


      
        public ServiceResponse<List<Dictionary<string, object>>> GetWidgetData(Widget widgetInfo)
        {
            if (widgetInfo != null)
            {
                string connectionString = widgetInfo.CONNCETIONSTRING;

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    string errorGroupsMessage = string.Empty;
                    try
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand(widgetInfo.SQLCOMMAND, connection))
                        {
                            using (OracleDataReader reader = command.ExecuteReader())
                            {

                                DataTable dataTable = new DataTable();
                                dataTable.Load(reader);
                                
                                if (!string.IsNullOrEmpty(widgetInfo.GROUPCOLUMN))
                                {
                                    List<string> errorGroups = ValidateWidget(dataTable, widgetInfo);
                                    if (errorGroups.Count > 0)
                                    {
                                        foreach (var group in errorGroups)
                                        {
                                            errorGroupsMessage += group + ";";
                                        }
                                    }
                                }
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
                                    Data = rows,
                                    Message = errorGroupsMessage != string.Empty ? errorGroupsMessage : string.Empty
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred while executing the query: " + ex.Message);
                        return new ServiceResponse<List<Dictionary<string, object>>>()
                        {
                            Data = null,
                            Message = ex.Message,
                            Success = false
                        };
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public List<string> ValidateWidget(DataTable widgetData, Widget widgetInfo)
        {
            List<string> result = new List<string>();
            var groupedData = widgetData.AsEnumerable()
                    .GroupBy(row => row.Field<string>(widgetInfo.GROUPCOLUMN));
            foreach (var group in groupedData)
            {
                var distinctValues = group.Select(row => row.Field<decimal>(widgetInfo.VALUECOLMN)).Distinct();
                if (distinctValues.Count() > 1)
                {
                    result.Add(group.Key);
                }
            }
            return result;
        }

        public ServiceResponse<List<Dictionary<string, object>>> GetStatusWidgetData(Widget widgetInfo)
        {
            if (widgetInfo != null)
            {
                string connectionString = widgetInfo.CONNCETIONSTRING;

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
                                    Data = rows,
                                    Message = "Data retrived susccessfully"
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred while executing the query: " + ex.Message);
                        return new ServiceResponse<List<Dictionary<string, object>>>()
                        {
                            Data = null,
                            Message = ex.Message,
                            Success = false
                        };
                    }
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<ServiceResponse<List<ClientWidget>>> GetWidgetsInfo()
        {
            try
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
            catch (Exception ex)
            {
                return new ServiceResponse<List<ClientWidget>>()
                {
                    Data = null,
                    Message = ex.Message,
                    Success = false
                };
            }
        }

        public async Task<ServiceResponse<ClientWidget>> UpdateWidget(ClientWidget widget)
        {
            try
            {
                _context.Widgets.Update(widget.WidgetInfo);
                await _context.SaveChangesAsync();
                return new ServiceResponse<ClientWidget>() { 
                Data = widget,
                Message = "Widget Updated",
                Success = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ClientWidget>()
                {
                    Data = null,
                    Message = ex.Message,
                    Success = false
                };
            }
        }

        public async Task<ServiceResponse<ClientWidget>> AddWidget(ClientWidget widget)
        {
            try
            {
                await _context.Widgets.AddAsync(widget.WidgetInfo);
                await _context.SaveChangesAsync();
                return new ServiceResponse<ClientWidget>()
                {
                    Data = widget,
                    Message = "Widget Added",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ClientWidget>()
                {
                    Data = null,
                    Message = ex.Message,
                    Success = false
                };
            }
        }
        
        public async Task<ServiceResponse<ClientWidget>> RemoveWidget(ClientWidget widget)
        {
            try
            {
                 _context.Widgets.Remove(widget.WidgetInfo);
                await _context.SaveChangesAsync();
                return new ServiceResponse<ClientWidget>()
                {
                    Data = widget,
                    Message = "Widget Removed",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ClientWidget>()
                {
                    Data = null,
                    Message = ex.Message,
                    Success = false
                };
            }
        }
    }
}
