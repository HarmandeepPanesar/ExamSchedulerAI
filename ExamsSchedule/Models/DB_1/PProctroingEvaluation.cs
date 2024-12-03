using System;
using System.Collections.Generic;

namespace ExamsSchedule.Models.DB
{
    public partial class PProctroingEvaluation
    {
        public int Id { get; set; }
        public long ProctoringStatuId { get; set; }
        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public int ProctoringStatus { get; set; }
        public string Comments { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
