using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Models
{
    public class ExamUploads
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Status { get; set; }
    }
}
