using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGXMonitoring.Shared
{
    [Table("USERS")]
    public class UserWidgets
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int USERID { get; set; }
        public int WIDGETID { get; set; }
        public int ISACTIVE { get; set; }
        public DateTime CREATEDON { get; set; }
    }
}
