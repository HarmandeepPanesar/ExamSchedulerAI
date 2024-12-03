using System;
using System.Collections.Generic;

namespace ExamsSchedule.Models.DB
{
    public partial class PStudentExamPicture
    {
        public int StudentPictureId { get; set; }
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public byte[] Picture { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}
