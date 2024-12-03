//using ExamsSchedule.Pages;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Threading.Tasks;

//namespace ExamsSchedule.Models
//{
//    public class Student
//    {
//        [Key]
//        public int Id { get; set; }
//        [Required]
//        public string FirstName { get; set; }
//        [Required]
//        public string MiddleName { get; set; }
//        [Required]
//        public string LastName { get; set; }
//        [Required]
//        public DateTime DoB { get; set; } = DateTime.Now;
//        [Required]
//        public string StudentIdentity { get; set; }
//        [Required]
//        public string StudentUniqueId { get; set; }
//        [Required]
//        public bool Active { get; set; } = true;
//        //[Required]
//        public DateTime CreatedDate { get; set; } = DateTime.Now;
//        //[Required]
//        public string CreatedBy { get; set; }
//        public ICollection<ExamStudent> ExamStudents { get; set; }
//    }
//}
