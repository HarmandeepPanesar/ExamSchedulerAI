using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamsSchedule.Models.DB
{
    public partial class PExamStudentProctoringStatus
    {
        public PExamStudentProctoringStatus()
        {
            PProctroingEvaluation = new HashSet<PProctroingEvaluation>();
        }

        public long Id { get; set; }
        public int ExamId { get; set; }
        public string SessionId { get; set; }
        public int StudentId { get; set; }
        public int? ExamStatus { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string Uniqueidentifier { get; set; }
        public short Submitted { get; set; }
        public bool LiveVideo { get; set; }

        public virtual PExam Exam { get; set; }
        public virtual PStudent Student { get; set; }
        public virtual ICollection<PProctroingEvaluation> PProctroingEvaluation { get; set; }

        [NotMapped]
        public bool PhoneDetected { get; set; }
        [NotMapped]
        public bool PersonMissing { get; set; }
        [NotMapped]
        public bool MultiPerson { get; set; }
    }
}
