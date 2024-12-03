using System;
using System.Collections.Generic;

namespace ExamsSchedule.Models.DB
{
    public partial class PExamStudentVerification
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public int StepId { get; set; }
        public string Ipaddress { get; set; }
        public string MachineName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
