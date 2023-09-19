using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGXMonitoring.Shared.DTOS
{
    public class MailEvent
    {
        public bool MailSent { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public DateTime SentOn { get; set; }
    }
}
