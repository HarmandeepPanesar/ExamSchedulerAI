using ExamsSchedule.Models.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Models
{
    public class pLogs
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public bool CameraAccessFailed { get; set; }
        public string Message { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual PExam Exam { get; set; }
        public virtual PStudent Student { get; set; }
    }
}
