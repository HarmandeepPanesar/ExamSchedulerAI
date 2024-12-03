//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ExamsSchedule.Models
//{
//    public class ExamStudent
//    {
//        [Key]
//        public int Id { get; set; }
//        [ForeignKey("Student")]
//        public int ExamStudentId { get; set; }
//        public int ExamId { get; set; }
//        public bool Active { get; set; }

//        public Exam Exam { get; set; }
//        public Student Student { get; set; }
//    }
//}
