using System;
using System.Collections.Generic;

namespace ExamsSchedule.Models.DB
{
    public partial class PStudentFaculty
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int StudentId { get; set; }
    }
}
