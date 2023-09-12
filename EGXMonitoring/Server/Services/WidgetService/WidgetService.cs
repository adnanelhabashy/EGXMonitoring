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
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

namespace EGXMonitoring.Server.Services.WidgetService
{
    public class WidgetService : IWidgetService
    {
        private readonly DataContext _context;
        private byte[] key;
        private byte[] iv;

        public WidgetService(DataContext context)
        {
            _context = context;
        }

        public ServiceResponse<List<Dictionary<string, object>>> GetWidgetData(ClientWidget widgetInfo)
        {
            if (widgetInfo != null)
            {
                string connectionString = widgetInfo.ConnectionString;

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    string errorGroupsMessage = string.Empty;
                    try
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand(widgetInfo.WidgetInfo.SQLCOMMAND, connection))
                        {
                            using (OracleDataReader reader = command.ExecuteReader())
                            {

                                DataTable dataTable = new DataTable();
                                dataTable.Load(reader);

                                if (!string.IsNullOrEmpty(widgetInfo.WidgetInfo.GROUPCOLUMN))
                                {
                                    List<string> errorGroups = ValidateWidget(dataTable, widgetInfo.WidgetInfo);
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

        public ServiceResponse<List<Dictionary<string, object>>> GetStatusWidgetData(ClientWidget widgetInfo)
        {
            if (widgetInfo != null)
            {
                string connectionString = widgetInfo.ConnectionString;

                using (OracleConnection connection = new OracleConnection(connectionString))
                {

                    try
                    {
                        connection.Open();

                        using (OracleCommand command = new OracleCommand(widgetInfo.WidgetInfo.SQLCOMMAND, connection))
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
                        ConnectionString = Decrypt(widgetInfo.CONNCETIONSTRINGHASH)
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
                return new ServiceResponse<ClientWidget>()
                {
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
            widget.WidgetInfo.CONNCETIONSTRINGHASH = Encrypt(widget.ConnectionString);
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




        public string Encrypt(string text)
        {
            string key = "b14ba5848a4e4833rbye2ee2375a5918";

            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(text);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array).Replace('/', '*').Replace('+', ';');
        }

        public string Decrypt(string encryptedText)
        {
            string key = "b14ba5848a4e4833rbye2ee2375a5918";

            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(encryptedText.Replace('*', '/').Replace(';', '+'));

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
