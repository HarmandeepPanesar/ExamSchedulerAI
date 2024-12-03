using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExamsSchedule.Models.DB
{
    public partial class Lmsintegration
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
