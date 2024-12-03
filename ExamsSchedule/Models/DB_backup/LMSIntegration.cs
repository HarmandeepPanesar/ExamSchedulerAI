using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Models.DB
{
    public class LMSIntegration
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "LMS Web Service Link")]
        public string WebServiceLink { get; set; }
        [Required]
        [Display(Name = "Security Token")]
        public string SecurityToken { get; set; }
    }
}
