using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGXMonitoring.Shared
{
    [Table("USERS")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string USERNAME { get; set; } = string.Empty;    
        public byte[]? PASSWORDHASH { get; set; }
        public byte[]? PASSWORDSALT { get; set; }
        public int ISACTIVE { get; set; } = 1;
        public DateTime CREATEDON { get; set; }
        public int ISADMIN { get; set; } = 0;
        public int RESETPASSWORD { get; set; } = 0;
        public DateTime UPDATEDON { get; set; }

    }
}
