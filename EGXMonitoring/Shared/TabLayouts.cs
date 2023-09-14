using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGXMonitoring.Shared
{
    [Table("TABLAYOUTS")]
    public class TabLayouts
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal ID { get; set; }
        public decimal USERID { get; set; }
        public string TABNAME { get; set; } = string.Empty;
        public string TABSTATE { get; set; } = string.Empty;
    }
}
