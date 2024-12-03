using ExamsSchedule.Data;
using ExamsSchedule.Models;
using ExamsSchedule.Models.DB;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Services
{
    public class ExamStudentService
    {
        private readonly DeepsensorClientContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        public ExamStudentService(DeepsensorClientContext dbContext, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<List<PExamStudent>> GetAllDataAsync(int studentId)
        {
            var examstudent = await _dbContext.PExamStudent.Where(m => m.StudentId == studentId).ToListAsync();
            return examstudent;
        }

        public async Task<List<PExamStudent>> GetAllByExamAsync(int examId)
        {
            var examstudent = await _dbContext.PExamStudent.Include(m => m.Student).Where(m => m.ExamId == examId).ToListAsync();
            return examstudent;
        }

        public async Task AddExamStudentAsync(int studentId, IEnumerable<int> values)
        {
            //GET STUDENT EXAMS
            var examstudent = await _dbContext.PExamStudent.Where(m => m.StudentId == studentId).ToListAsync();
            var previousExamIds = examstudent.Select(m => m.ExamId).ToList();
            var selectedExamIds = values.ToList();

            //GET EXAMS TO DELETE
            var removedExamsIds = previousExamIds.Except(selectedExamIds);
            var studentExamsToRemove = examstudent.Where(m => removedExamsIds.Contains(m.ExamId)).ToList();
            _dbContext.PExamStudent.RemoveRange(studentExamsToRemove);
            await _dbContext.SaveChangesAsync();

            //REMOVE MAPPING FROM STUDENT FACULTY IF EXAM REMOVED FROM STUDENT
            var examstudentId = studentExamsToRemove.Select(m => m.ExamStudentId).ToList();
            var sFaculty = await _dbContext.PStudentFaculty.Where(m => examstudentId.Contains(m.ExamStudentId)).ToListAsync();
            _dbContext.PStudentFaculty.RemoveRange(sFaculty);
            await _dbContext.SaveChangesAsync();

            //ADD NEWLY ADDED EXAMS
            var newToAddIds = selectedExamIds.Except(previousExamIds);

            foreach (var item in values.Where(m => newToAddIds.Contains(m)).ToList())
            {
                var result = new PExamStudent
                {
                    StudentId = studentId,
                    ExamId = item
                };
                await _dbContext.PExamStudent.AddAsync(result);
                await _dbContext.SaveChangesAsync();
            }
        }

        //Read Excel File
        public async Task<List<StudentExamUploadFile>> ImportData(IBrowserFile imageFile)
        {
            List<StudentExamUploadFile> studentExamfile = new List<StudentExamUploadFile>();
            var stream = imageFile.OpenReadStream();
            using (var stream2 = new MemoryStream())
            {
                await stream.CopyToAsync(stream2);   // although file.Data is itself a stream, using it directly causes "synchronous reads are not supported" errors below.
                stream2.Seek(0, SeekOrigin.Begin);      // at the end of the copy method, we are at the end of both the input and output stream and need to reset the one we want to work with.
                var sreader = new System.IO.StreamReader(stream2);

                string[] headers = sreader.ReadLine().Split(',');     //Title
                while (!sreader.EndOfStream)                          //get all the content in rows 
                {
                    string[] rows = sreader.ReadLine().Split(',');
                    string Id = rows[0].ToString();
                    string NUM = rows[1].ToString();

                    StudentExamUploadFile studentExamUploadFile = new StudentExamUploadFile();
                    studentExamUploadFile.ExameName = rows[0].ToString();
                    studentExamUploadFile.ExamIdentifier1 = rows[1].ToString();
                    studentExamUploadFile.ExamIdentifier2 = rows[2].ToString();
                    studentExamUploadFile.StudentEmail = rows[3].ToString();
                    studentExamUploadFile.StudentUniqueId = rows[4].ToString();
                    studentExamUploadFile.FirstName = rows[5].ToString();
                    studentExamUploadFile.LastName = rows[6].ToString();
                    studentExamUploadFile.StudentIdentity = rows[7].ToString();
                    //studentExamUploadFile.StudentDOB = Convert.ToDateTime(rows[8].ToString());
                    studentExamfile.Add(studentExamUploadFile);
                }
            }

            foreach (var item in studentExamfile)
            {
                //CHECK IF STUDEENT ALREADY EXIST
                var studemail = await _dbContext.PStudent.Where(m => !m.IsDeleted && m.Email == item.StudentEmail).FirstOrDefaultAsync();

                //CHECK IF EXAM ALREADY EXIST
                var examname = await _dbContext.PExam.Where(m => !m.IsDeleted && m.ExameName == item.ExameName).FirstOrDefaultAsync();

                //ADD EXAM IF NOT EXIST
                if (examname == null)
                {
                    examname = new PExam
                    {
                        ExameName = item.ExameName,
                        ExamDescription = item.ExameName,
                        ExamIdentifier1 = item.ExamIdentifier1,
                        ExamIdentifier2 = item.ExamIdentifier2,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(1)
                    };
                    await _dbContext.PExam.AddAsync(examname);
                }

                //ADD STUDENT IF NOT EXIST
                if (studemail == null)
                {
                    studemail = new PStudent
                    {
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        DoB = DateTime.Now,
                        StudentIdentity = item.StudentIdentity,
                        StudentUniqueId = item.StudentUniqueId,
                        Active = item.Active,
                        CreatedDate = DateTime.Now,
                        CreatedBy = "1",
                        Email = item.StudentEmail,
                    };
                    await _dbContext.PStudent.AddAsync(studemail);
                }
                await _dbContext.SaveChangesAsync();

                //ADD STUDENT EXAM MAPPING
                var examnamemap = await _dbContext.PExamStudent.Where(m => m.ExamId == examname.ExamId && m.StudentId == studemail.Id).FirstOrDefaultAsync();
                if (examnamemap == null)
                {
                    await _dbContext.PExamStudent.AddAsync(new PExamStudent
                    {
                        ExamId = examname.ExamId,
                        StudentId = studemail.Id
                    });
                    await _dbContext.SaveChangesAsync();
                }
            }
            return studentExamfile;
        }
 
    }
}
