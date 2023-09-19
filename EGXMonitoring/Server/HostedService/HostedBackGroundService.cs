using EGXMonitoring.Client.Services.SoundService;
using EGXMonitoring.Shared;
using EGXMonitoring.Shared.DTOS;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Timers;
using Microsoft.Exchange.WebServices.Data;
using System.Diagnostics;

namespace EGXMonitoring.Server.HostedService
{
    public class HostedBackGroundService : IHostedService, IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly IWidgetService _WidgetService;
        private readonly List<System.Threading.Tasks.Task> runningTasks;
        private readonly IHost _applicationLifetime;

        public HostedBackGroundService(IServiceScopeFactory factory, IHost applicationLifetime)
        {
            _WidgetService = factory.CreateScope().ServiceProvider.GetRequiredService<IWidgetService>();
            cancellationTokenSource = new CancellationTokenSource();
            runningTasks = new List<System.Threading.Tasks.Task>();
            _applicationLifetime = applicationLifetime;

        }
        public async System.Threading.Tasks.Task StartAsync(CancellationToken cancellationToken)
        {
            ServiceResponse<List<ClientWidget>> Widgets = await _WidgetService.GetWidgetsInfo();



            foreach (ClientWidget widget in Widgets.Data)
            {
                if (widget.WidgetInfo.WIDGETTYPE == 1)
                {
                    System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Run(() => ProcessSyncWidget(widget, cancellationTokenSource.Token));
                    runningTasks.Add(task);
                }
                if (widget.WidgetInfo.WIDGETTYPE == 2)
                {
                    System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Run(() => ProcessStatusWidget(widget, cancellationTokenSource.Token));
                    runningTasks.Add(task);
                }
                // Start a task for each widget to run the method with the specified refresh interval

            }


        }
        public System.Threading.Tasks.Task StopAsync(CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public void Dispose()
        {
            cancellationTokenSource.Dispose();
        }

        private async System.Threading.Tasks.Task ProcessSyncWidget(ClientWidget widget, CancellationToken cancellationToken)
        {
            DataTable Data = new DataTable();
            Dictionary<string, DateTime> ErrorTime = new Dictionary<string, DateTime>();
            List<string> errors = new List<string>();
            bool mailSent = false;
            Console.WriteLine(widget.WidgetInfo.NAME + " -- before loop");
            List<MailEvent> mailEvents = new List<MailEvent>();

            while (!cancellationToken.IsCancellationRequested)
            {
                var result = _WidgetService.GetWidgetData(widget);
                Data = ManipulateData(result.Data);
                errors = result.Success ? result.Message.Split(";").ToList() : new List<string>();

                if (result.Success)
                {
                    Console.WriteLine(widget.WidgetInfo.NAME + " -- " + widget.WidgetInfo.REFRESHINTERVAL + " -- Check type 1 ");

                    var groupedData = Data.AsEnumerable()
                                .GroupBy(row => row.Field<string>(widget.WidgetInfo.GROUPCOLUMN));
                    foreach (var group in groupedData)
                    {
                        if (errors.Contains(group.Key))
                        {
                            var lowestValue = group.Min(row => row.Field<string>(widget.WidgetInfo.VALUECOLMN));
                            string warningLevel = string.Empty;
                            Console.WriteLine(widget.WidgetInfo.NAME + " -- " + lowestValue + " -- Lowest value");
                            if (!ErrorTime.ContainsKey(group.Key))
                            {
                                ErrorTime[group.Key] = DateTime.Now;
                            }
                            if (widget.WidgetInfo.CHECKTYPE == 1)
                            {
                                warningLevel = ProcessWarning(ErrorTime[group.Key], group.Key, widget.WidgetInfo.ALARMAFTER);
                                if (warningLevel == "4")
                                {
                                    if(mailEvents.Where(m => m.GroupName == group.Key && m.MailSent == true).Count() == 0)
                                    {
                                        sendmail("adnan.ahmed@egx.com.eg", "error in " + widget.WidgetInfo.NAME + " in " + group.Key + "not in sync since " + ErrorTime[group.Key].ToString(), "error in " + widget.WidgetInfo.NAME + " in " + group.Key);

                                        MailEvent sentMail = new MailEvent()
                                        {
                                            GroupName = group.Key,
                                            SentOn = DateTime.Now,
                                            MailSent = true
                                        };
                                        mailEvents.Add(sentMail);
                                    }
                                   
                                }
                            }
                            if (widget.WidgetInfo.CHECKTYPE == 2)
                            {
                                DateTime currentDateTime = DateTime.Now;
                                if (widget.WidgetInfo.AFTERTIME.HasValue && currentDateTime.TimeOfDay > widget.WidgetInfo.AFTERTIME.Value.TimeOfDay && currentDateTime.TimeOfDay < widget.WidgetInfo.ENDTIME.Value.TimeOfDay)
                                {
                                    warningLevel = ProcessWarning(ErrorTime[group.Key], group.Key, widget.WidgetInfo.ALARMAFTER);
                                    if (warningLevel == "4" )
                                    {
                                        if (mailEvents.Where(m => m.GroupName == group.Key && m.MailSent == true).Count() == 0)
                                        {
                                            sendmail("adnan.ahmed@egx.com.eg", "error in " + widget.WidgetInfo.NAME + " in " + group.Key + " not in sync since " + ErrorTime[group.Key].ToString(), "error in " + widget.WidgetInfo.NAME + " in " + group.Key);

                                            MailEvent sentMail = new MailEvent()
                                            {
                                                GroupName = group.Key,
                                                SentOn = DateTime.Now,
                                                MailSent = true
                                            };
                                            mailEvents.Add(sentMail);
                                        }
                                    }
                                }
                            }
                            Console.WriteLine(widget.WidgetInfo.NAME + " -- Error time Captured - wanring level - " + warningLevel);
                        }
                        else
                        {
                            if (ErrorTime.ContainsKey(group.Key))
                            {
                                ErrorTime = RemoveErrorTime(@group.Key, ErrorTime);
                            }
                            if (mailEvents.Where(m => m.GroupName == group.Key && m.MailSent == true).Count() > 0)
                            {
                                mailEvents.RemoveAll(m => m.GroupName == group.Key);
                            }
                        }
                    }

                }


                Console.WriteLine(widget.WidgetInfo.NAME + " -- " + widget.WidgetInfo.REFRESHINTERVAL + " -- Data Reriveied");
                await System.Threading.Tasks.Task.Delay(widget.WidgetInfo.REFRESHINTERVAL * 1000, cancellationToken);
            }
        }


        private async System.Threading.Tasks.Task ProcessStatusWidget(ClientWidget widget, CancellationToken cancellationToken)
        {
            DataTable Data = new DataTable();
            Dictionary<string, DateTime> ErrorTime = new Dictionary<string, DateTime>();
            List<string> errors = new List<string>();
            bool mailSent = false;
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = _WidgetService.GetStatusWidgetData(widget);
                if (result.Success)
                {
                    Data = ManipulateData(result.Data);
                    foreach (DataRow row in Data.Rows)
                    {
                        if (row[widget.WidgetInfo.VALUECOLMN].ToString() == "-1")
                        {
                            string warningLevel = string.Empty;
                            if (!ErrorTime.ContainsKey(row[widget.WidgetInfo.VALUECOLMN].ToString()))
                            {
                                ErrorTime[row[widget.WidgetInfo.VALUECOLMN].ToString()] = DateTime.Now;
                            }


                            if (widget.WidgetInfo.CHECKTYPE == 1)
                            {
                                warningLevel = ProcessWarning(ErrorTime[row[widget.WidgetInfo.VALUECOLMN].ToString()], row[widget.WidgetInfo.GROUPCOLUMN].ToString(), widget.WidgetInfo.ALARMAFTER);
                                Console.WriteLine(widget.WidgetInfo.NAME + " -- Error time Captured - wanring level - " + warningLevel);
                            }
                            if (widget.WidgetInfo.CHECKTYPE == 2)
                            {
                                DateTime currentDateTime = DateTime.Now;
                                if (widget.WidgetInfo.AFTERTIME.HasValue && currentDateTime.TimeOfDay > widget.WidgetInfo.AFTERTIME.Value.TimeOfDay && currentDateTime.TimeOfDay < widget.WidgetInfo.ENDTIME.Value.TimeOfDay)
                                {
                                    warningLevel = ProcessWarning(ErrorTime[row[widget.WidgetInfo.VALUECOLMN].ToString()], row[widget.WidgetInfo.GROUPCOLUMN].ToString(), widget.WidgetInfo.ALARMAFTER);
                                    Console.WriteLine(widget.WidgetInfo.NAME + " -- Error time Captured - wanring level - " + warningLevel);
                                }
                            }
                        }
                        else
                        {
                            ErrorTime = RemoveErrorTime(row[widget.WidgetInfo.VALUECOLMN].ToString(), ErrorTime);
                        }
                    }

                }
                Console.WriteLine(widget.WidgetInfo.NAME + " -- " + widget.WidgetInfo.REFRESHINTERVAL);
                await System.Threading.Tasks.Task.Delay(widget.WidgetInfo.REFRESHINTERVAL * 1000, cancellationToken);
            }
        }


        private DataTable ManipulateData(List<Dictionary<string, object>> dataList)
        {
            DataTable result = new DataTable();
            foreach (var key in dataList[0].Keys)
            {
                result.Columns.Add(key);
            }

            // Add rows to DataTable
            foreach (var dict in dataList)
            {
                DataRow row = result.NewRow();

                foreach (var pair in dict)
                {
                    row[pair.Key] = pair.Value.ToString();
                }

                result.Rows.Add(row);
            }

            return result;
        }

        private string ProcessWarning(DateTime time, string groupname, int? minutes)
        {
            int? Level1 = 0;
            int? Level2 = 0;
            int? Level3 = 0;

            int? milseconds = minutes * 60 * 1000;

            Level1 = milseconds / 3;
            Level2 = Level1 * 2;
            Level3 = Level1 * 3;

            DateTime currentTime = DateTime.Now;
            DateTime parsedTime = time;
            TimeSpan timeDifference = parsedTime - currentTime;
            long timeDifferenceMilliseconds = (long)timeDifference.TotalMilliseconds * -1;


            if (timeDifferenceMilliseconds >= 0 && timeDifferenceMilliseconds <= Level1)
            {

                return "1";
            }
            else if (timeDifferenceMilliseconds >= Level1 && timeDifferenceMilliseconds <= Level2)
            {
                return "2";
            }
            else if (timeDifferenceMilliseconds >= Level2 && timeDifferenceMilliseconds <= Level3)
            {
                return "3";
            }
            else
            {
                return "4";
            }
        }

        private bool SearchErrorTime(string groupname, Dictionary<string, DateTime> ErrorTime)
        {
            bool containsKey = ErrorTime.ContainsKey(groupname);
            return containsKey;
        }

        private DateTime GetErrortime(string groupname, Dictionary<string, DateTime> ErrorTime)
        {
            return ErrorTime[groupname];
        }

        private Dictionary<string, DateTime> RemoveErrorTime(string groupname, Dictionary<string, DateTime> ErrorTime)
        {
            bool removed = ErrorTime.Remove(groupname);
            return ErrorTime;
        }


        public async System.Threading.Tasks.Task RestartAsync()
        {
            await _applicationLifetime.StopAsync();

            // Give the application some time to gracefully stop
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(5));

            // Resume the application
            await _applicationLifetime.StartAsync();
        }

        private void sendmail(string to, string body, string subject)
        {

            ExchangeService exchangeService = new ExchangeService(ExchangeVersion.Exchange2016);
            exchangeService.Credentials = new WebCredentials("egx\\adnan.ahmed", "Ad@852951753951");
            exchangeService.Url = new Uri("https://mail.egx.com.eg/EWS/Exchange.asmx");
            exchangeService.TraceEnabled = true;
            exchangeService.TraceFlags = TraceFlags.All;
            EmailMessage email = new EmailMessage(exchangeService);
            email.Subject = subject;
            email.Body = new MessageBody(body);
            email.ToRecipients.Add(to);
            email.SendAndSaveCopy();

            Console.WriteLine("Mails sent");
        }
    }

  
}
