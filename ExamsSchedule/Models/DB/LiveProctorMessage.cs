using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Models.DB
{
    public class LiveProctorMessage
    {
        public int Id { get; set; }
        [Required]
        public string Message { get; set; }
        [ForeignKey("User")]
        public string SendBy { get; set; }
        public DateTime SendDate { get; set; } = DateTime.UtcNow;
        public int Status { get; set; }
        public long ProctoringStatusId { get; set; }
        public virtual AspNetUsers User { get; set; }
        public int WarningType { get; set; }
        public PExamStudentProctoringStatus ProctoringStatus { get; set; }

        [NotMapped]
        public string SendByUser => User?.Email;

        [NotMapped]
        public string Student => ProctoringStatus?.Student?.LastName + " " + ProctoringStatus?.Student?.FirstName;

        [NotMapped]
        public string Exam => ProctoringStatus?.Exam?.ExameName;

        [NotMapped]
        public string MessageStatus => GetStatus();

        [NotMapped]
        public string MessageWarningType => GetWarningType();


        public string GetStatus()
        {
            if (Status == 0)
            {
                return "Pending ";
            }
            else if (Status == 1)
            {
                return "Read ";
            }
            else if (Status == 3)
            {
                return "Approve ";
            }
            return "";
        }

        public string GetWarningType()
        {
            if (WarningType == 1)
            {
                return "Red";
            }
            else if (WarningType == 2)
            {
                return "Orange ";
            }
            else
            {
                return "Yellow";
            }
        }
    }

}
