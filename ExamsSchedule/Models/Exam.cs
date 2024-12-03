//using ExamsSchedule.Pages;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ExamsSchedule.Models
//{
//    public class Exam
//    {
//        [Key]
//        public int Id { get; set; }
//        [Required]
//        public string ExamName { get; set; }
//        [Required]
//        public string ExamDesciption { get; set; }
//        [Required]
//        public string ExamIdentifier1 { get; set; }
//        [Required]
//        public string ExamIdentifier2 { get; set; }
//        [Required]
//        public DateTime StartDate { get; set; } = DateTime.Now;
//        [Required]
//        public DateTime EndDate { get; set; } = DateTime.Now;
//        [Required]
//        public bool Active { get; set; } = true;
//        public ICollection<ExamStudent> ExamStudents { get; set; }
//    }
//}
