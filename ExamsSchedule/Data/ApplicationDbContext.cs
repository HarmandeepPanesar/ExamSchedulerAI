using System;
using System.Collections.Generic;
using System.Text;
using ExamsSchedule.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExamsSchedule.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //public DbSet<Student> Students { get; set; }
        //public DbSet<Exam> Exams { get; set; }
        //public DbSet<ExamStudent> ExamStudents { get; set; }

    }
}
