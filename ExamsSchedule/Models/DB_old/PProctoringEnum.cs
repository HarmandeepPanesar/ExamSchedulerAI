using System;
using System.Collections.Generic;

namespace ExamsSchedule.Models.DB
{
    public partial class PProctoringEnum
    {
        public int Id { get; set; }
        public string ProctoringStatus { get; set; }
        public string Description { get; set; }
        public int EnumValue { get; set; }
    }
}
