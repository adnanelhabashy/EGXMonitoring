using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGXMonitoring.Shared
{
    [Table("WIDGETS")]
    public class Widget
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string NAME { get; set; } = string.Empty;
        public int REFRESHINTERVAL { get; set; }
        public int NOOFCOLUMNS { get; set; }
        public string SQLCOMMAND { get; set; } = string.Empty;
        public string CONNCETIONSTRINGID { get; set; } = string.Empty;
        public string CONDITION { get; set; } = string.Empty;
        public decimal? WIDGETTYPE { get; set; }
        public string? NAMECOLUMN { get; set; } = string.Empty;
        public string? VALUECOLMN { get; set; } = string.Empty;

    }
}
