using System;
using System.Collections.Generic;

namespace ExamsSchedule.Models.DB
{
    public partial class PExamStudent
    {
        public int ExamStudentId { get; set; }
        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public bool Active { get; set; }

        public virtual PExam Exam { get; set; }
        public virtual PStudent Student { get; set; }
    }
}
