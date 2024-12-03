using System;
using System.Collections.Generic;

namespace ExamsSchedule.Models.DB
{
    public partial class ProctoringQueue
    {
        public long Id { get; set; }
        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public string OrgCode { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public DateTime? DateTimeInserted { get; set; }
        public DateTime? DateTimeProcessed { get; set; }
        public DateTime? DateTimePicked { get; set; }
    }
}
