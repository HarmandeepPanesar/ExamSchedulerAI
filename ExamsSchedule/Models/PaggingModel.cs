using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Models
{
    public class PaggingModel
    {
        public int PageNumber { get; set; }
        public bool IsSelected { get; set; }
    }
}
