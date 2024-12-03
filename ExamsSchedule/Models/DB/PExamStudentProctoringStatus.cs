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

        public int? VideoStatus { get; set; }

        public bool? ImpersonationCheckPassed { get; set; }
        public bool? InitialFaceVerificationPassed { get; set; }
        public DateTime? LastVerificationDone { get; set; }

        public DateTime? LastEventDetected { get; set; }

        public string ReferenceImage { get; set; }
        public bool NeedAssistance { get; set; } = false;
        public virtual PExam Exam { get; set; }
        public virtual PStudent Student { get; set; }
        public virtual ICollection<PProctroingEvaluation> PProctroingEvaluation { get; set; }

        public int? Eventcount { get; set; }
        //[NotMapped]
        public bool? PhoneDetected { get; set; }
        //[NotMapped]
        //public bool PersonMissing { get; set; }
        //[NotMapped]
        //public bool MultiPerson { get; set; }
        //[NotMapped]
        //public bool LookingAway { get; set; }
        public bool? NoPersonDetected { get; set; }
        public bool? MultiPersonDetected { get; set; } 
        public bool? EyeGazeDetected { get; set; }
        public string StudentLatestImage { get; set; }
        [NotMapped]
        public bool IsMessageAvaiable { get; internal set; }
        [NotMapped]
        public int? MessageStatus { get; set; }
        [NotMapped]
        public bool EnableChat { get; set; }
    }
}
