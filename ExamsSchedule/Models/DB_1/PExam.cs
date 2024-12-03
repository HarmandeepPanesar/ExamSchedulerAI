using System;
using System.Collections.Generic;

namespace ExamsSchedule.Models.DB
{
    public partial class PExam
    {
        public PExam()
        {
            PExamStudent = new HashSet<PExamStudent>();
        }

        public int ExamId { get; set; }
        public string ExameName { get; set; }
        public string ExamDescription { get; set; }
        public string ExamIdentifier1 { get; set; }
        public string ExamIdentifier2 { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<PExamStudent> PExamStudent { get; set; }
    }
}
