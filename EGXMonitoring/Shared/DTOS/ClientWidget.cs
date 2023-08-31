using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EGXMonitoring.Shared.DTOS
{
    public class ClientWidget
    {
        public Widget WidgetInfo { get; set; } = new Widget();
        public List<Dictionary<string, object>> Data { get; set; } =   new List<Dictionary<string, object>> ();
    }
}
