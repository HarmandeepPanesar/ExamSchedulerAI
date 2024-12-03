using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamsSchedule.Models.DB
{
    public partial class PStudent
    {
        public PStudent()
        {
            PExamStudent = new HashSet<PExamStudent>();
            PExamStudentProctoringStatus = new HashSet<PExamStudentProctoringStatus>();
        }

        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public DateTime DoB { get; set; } = DateTime.Now.AddYears(-15);
        [Required]
        [Display(Name = "Student Identity")]
        public string StudentIdentity { get; set; }
        [Required]
        [Display(Name = "Student Unique Id")]
        public string StudentUniqueId { get; set; }
        public bool Active { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; }

        [NotMapped]
        public string FullName => $"{LastName} {FirstName}";
        [NotMapped]
        public int ExamStudentId { get; set; }
        public virtual ICollection<PExamStudent> PExamStudent { get; set; }
        public virtual ICollection<PExamStudentProctoringStatus> PExamStudentProctoringStatus { get; set; }
    }
}
