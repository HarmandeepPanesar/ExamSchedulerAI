using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamsSchedule.Models.DB
{
    public partial class pExamStudentProctoringStatusLogs
    {
        public int Id { get; set; }
        public long ProctoringStatuId { get; set; }
        public string StatusId { get; set; }
        public DateTime CreatedDate { get; set; }

        [ForeignKey("User")]
        public string CreatedBy { get; set; }

        public virtual AspNetUsers User { get; set; }
    }
}
