using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Models.DB
{
    public class StudentImpersonation
    {
        public long Id { get; set; }
        public long ProctoringStatusId { get; set; }
        public string RefImage { get; set; }
        public string CompareImage { get; set; }
        public bool Match { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
