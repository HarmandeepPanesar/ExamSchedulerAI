using System;
using System.Collections.Generic;

namespace ExamsSchedule.Models.DB
{
    public partial class PEvents
    {
        public int EventId { get; set; }
        public int StudentId { get; set; }
        public string ExamIdentifier { get; set; }
        public string Event { get; set; }
        public string FileName { get; set; }

        public string SessionId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
