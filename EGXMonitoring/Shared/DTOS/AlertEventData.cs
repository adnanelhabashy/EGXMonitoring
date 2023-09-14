using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGXMonitoring.Shared.DTOS
{
    public class AlertEventData
    {
        public string Message { get; set; } =string.Empty;
        public int? Severity { get; set; }
    }
}
