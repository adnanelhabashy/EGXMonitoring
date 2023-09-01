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
        public string SQLCOMMAND { get; set; } = string.Empty;
        public string CONNCETIONSTRING { get; set; } = string.Empty;
        public decimal? WIDGETTYPE { get; set; }
        public string? VALUECOLMN { get; set; } = string.Empty;
        public string? GROUPCOLUMN { get; set; } = string.Empty;
        public decimal? CHECKTYPE { get; set; } 
    }
}
