using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExamsSchedule.Models.DB
{
    public partial class PExam
    {
        public PExam()
        {
            PExamStudent = new HashSet<PExamStudent>();
            PExamStudentProctoringStatus = new HashSet<PExamStudentProctoringStatus>();
        }

        public int ExamId { get; set; }
        [Required]
        [Display(Name = "Exame Name")]
        public string ExameName { get; set; }
        [Required]
        [Display(Name = "Exam Description")]
        public string ExamDescription { get; set; }
        [Required]
        [Display(Name = "Exam Identifier1")]
        public string ExamIdentifier1 { get; set; }
        [Required]
        [Display(Name = "Exam Identifier2")]
        public string ExamIdentifier2 { get; set; }
        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Now;
        [Required]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(15);
        public bool Active { get; set; } = true;

        public virtual ICollection<PExamStudent> PExamStudent { get; set; }
        public virtual ICollection<PExamStudentProctoringStatus> PExamStudentProctoringStatus { get; set; }
    }
}
