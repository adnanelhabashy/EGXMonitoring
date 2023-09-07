using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGXMonitoring.Shared.DTOS
{
    public class UserVM
    {
        public int ID { get; set; }
        public string USERNAME { get; set; } = string.Empty;
        public byte[] PASSWORDHASH { get; set; }
        public byte[] PASSWORDSALT { get; set; }
        public bool ISACTIVE { get; set; }
        public DateTime CREATEDON { get; set; }
        public bool ISADMIN { get; set; }
        public bool RESETPASSWORD { get; set; }
        public DateTime UPDATEDON { get; set;}
    }
}
