using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Models
{
    public class BulkStudentImageViewModel
    {
        public string Id { get; set; }
        public string ImageName { get; set; }
        public bool IsSaved { get; set; }

    }
}
