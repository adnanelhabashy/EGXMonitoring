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
        [Required(ErrorMessage = "The Widget Name is required.")]
        public string NAME { get; set; } = string.Empty;
        [Required(ErrorMessage = "The Refresh Interval is required.")]
        public int REFRESHINTERVAL { get; set; }
        [Required(ErrorMessage = "TheSQL Statement is required.")]
        public string SQLCOMMAND { get; set; } = string.Empty;
        public string CONNCETIONSTRINGHASH { get; set; } = string.Empty;
        [Required(ErrorMessage = "The Widget Type is required.")]
        public decimal? WIDGETTYPE { get; set; }
        [Required(ErrorMessage = "The Value Column is required.")]
        public string? VALUECOLMN { get; set; } = string.Empty;
        [Required(ErrorMessage = "The Group Column is required.")]
        public string? GROUPCOLUMN { get; set; } = string.Empty;
        [Required(ErrorMessage = "The Check Type is required.")]
        public decimal? CHECKTYPE { get; set; }
        [Required(ErrorMessage = "The Alarm After is required.")]

        public int? ALARMAFTER { get; set; }
        public DateTime? AFTERTIME { get; set; }
        [Required(ErrorMessage = "The Description is required.")]

        public string? DESCRIPTION { get; set; } = string.Empty;
        public DateTime? ENDTIME { get; set; }

        [Required(ErrorMessage = "The Severity is required.")]
        public int? SEVERITY { get; set; }
        


    }
}
