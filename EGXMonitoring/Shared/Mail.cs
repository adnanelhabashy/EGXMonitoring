using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGXMonitoring.Shared
{
    [Table("MAIL")]
    public class Mail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required(ErrorMessage = "The Widget Mail body is required.")]
        public string MAIL_BODY { get; set; } = string.Empty;
        [Required(ErrorMessage = "The Widget recipents is required.")]
        public string RECIPENTS { get; set; } = string.Empty;
    }
}
