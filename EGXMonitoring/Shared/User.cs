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
        public string PASSWORD { get; set; }= string.Empty;   
        public int ISACTIVE { get; set; }
        public DateTime CREATEDON { get; set; }
        public int ISADMIN { get; set; }
        public DateTime UPDATEDON { get; set; }

    }
}
