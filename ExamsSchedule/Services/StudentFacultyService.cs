using ExamsSchedule.Models;
using ExamsSchedule.Models.DB;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Services
{
    public class StudentFacultyService
    {
        private readonly DeepsensorClientContext _dbContext;
        private List<PStudentFaculty> result;
        private readonly UserManager<IdentityUser> _userManager;
        public LoginUploadViewModel facultyData = new LoginUploadViewModel();
        public StudentFacultyService(DeepsensorClientContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager,
           SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IPasswordHasher<IdentityUser> passwordHash)
        {
            _dbContext = dbContext;
            _userManager = userManager;

        }
        public StudentFacultyService(DeepsensorClientContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<IdentityUser>> GetAllDataAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("User");
            return users.ToList();
        }

        public async Task<List<PStudentFaculty>> GetDataAsync(IdentityUser identityUser, int examId, PStudent student)
        {
            var pStudentFaculty = _dbContext.PStudentFaculty.Include("ExamStudent.Student").Where(m => m.UserId == identityUser.Id);

            if (examId > 0)
                pStudentFaculty = pStudentFaculty.Where(m => m.ExamStudent.ExamId == examId);

            if (student != null)
                pStudentFaculty = pStudentFaculty.Where(m => m.ExamStudent.Student.FirstName == student.FirstName);

            return await pStudentFaculty.ToListAsync();

        }

        public async Task<List<PStudentFaculty>> AddstudentfacultyAsync(IdentityUser identityUser, IEnumerable<int> examstudentId, bool removeAndReinsert, int examId = 0)
        {
            //delete old mapping
            if (removeAndReinsert)
            {
                var studentFaculty = await _dbContext.PStudentFaculty.Where(m => m.UserId == identityUser.Id).ToListAsync();
                if (studentFaculty != null && studentFaculty.Count() > 0)
                {
                    if (examId > 0)
                    {
                        studentFaculty = studentFaculty.Where(m => m.ExamStudent.ExamId == examId).ToList();
                    }

                    _dbContext.PStudentFaculty.RemoveRange(studentFaculty);
                    await _dbContext.SaveChangesAsync();
                }
            }

            var examStudents = await _dbContext.PExamStudent.Where(m => m.ExamId == examId && examstudentId.Contains(m.StudentId)).ToListAsync();
            foreach (var item in examstudentId)
            {
                var examStudentId = examStudents.Where(m => m.StudentId == item).FirstOrDefault().ExamStudentId;
                var result = new PStudentFaculty
                {
                    UserId = identityUser.Id,
                    ExamStudentId = examStudentId
                };
                await _dbContext.PStudentFaculty.AddAsync(result);
                await _dbContext.SaveChangesAsync();
            }
            return result;
        }

        //Read Excel File
        public async Task<List<StudentExamUploadFile>> ImportData(IBrowserFile imageFile)
        {
            List<StudentExamUploadFile> studentexamfacultyUploadFiles = new List<StudentExamUploadFile>();
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

                    StudentExamUploadFile studentExamFacultyUploadFile = new StudentExamUploadFile();
                    studentExamFacultyUploadFile.ExameName = rows[0].ToString();
                    studentExamFacultyUploadFile.ExamIdentifier1 = rows[1].ToString();
                    studentExamFacultyUploadFile.ExamIdentifier2 = rows[2].ToString();
                    studentExamFacultyUploadFile.StudentEmail = rows[3].ToString();
                    studentExamFacultyUploadFile.StudentUniqueId = rows[4].ToString();
                    studentExamFacultyUploadFile.FirstName = rows[5].ToString();
                    studentExamFacultyUploadFile.LastName = rows[6].ToString();
                    studentExamFacultyUploadFile.StudentIdentity = rows[7].ToString();
                    //studentExamFacultyUploadFile.StudentDOB = Convert.ToDateTime(rows[8].ToString());
                    studentExamFacultyUploadFile.FacultyEmail = rows[8].ToString();
                    studentexamfacultyUploadFiles.Add(studentExamFacultyUploadFile);
                }
            }

            foreach (var item in studentexamfacultyUploadFiles)
            {
                var studentEmailLower = item.StudentEmail.ToLower();
                var studentData = await _dbContext.PStudent.Where(m => !m.IsDeleted && m.Email == studentEmailLower).FirstOrDefaultAsync();

                var examNameLower = item.ExameName.ToLower();
                var examData = await _dbContext.PExam.Where(m => !m.IsDeleted && m.ExameName == examNameLower).FirstOrDefaultAsync();
                var facultyData = await _userManager.FindByEmailAsync(item.FacultyEmail);

                if (examData == null)
                {
                    examData = new PExam
                    {
                        ExameName = item.ExameName,
                        ExamDescription = item.ExameName,
                        ExamIdentifier1 = item.ExamIdentifier1,
                        ExamIdentifier2 = item.ExamIdentifier2,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(1)
                    };
                    _dbContext.PExam.Add(examData);
                    await _dbContext.SaveChangesAsync();
                }

                if (studentData == null)
                {
                    studentData = new PStudent
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
                    _dbContext.PStudent.Add(studentData);
                    await _dbContext.SaveChangesAsync();
                }

                if (facultyData == null)
                {
                    var user = new IdentityUser
                    {
                        UserName = item.FacultyEmail,
                        Email = item.FacultyEmail
                    };
                    var result = await _userManager.CreateAsync(user, "123456");
                    if (result.Succeeded)
                    {
                        //get user to assign role
                        facultyData = await _userManager.FindByNameAsync(user.Email);
                        await _userManager.AddToRoleAsync(facultyData, "User");
                    }
                }

                var examnamemap = await _dbContext.PExamStudent.Where(m => m.ExamId == examData.ExamId && m.StudentId == studentData.Id).FirstOrDefaultAsync();
                if (examnamemap == null)
                {
                    _dbContext.PExamStudent.Add(new PExamStudent
                    {
                        ExamId = examData.ExamId,
                        StudentId = studentData.Id
                    });
                    await _dbContext.SaveChangesAsync();

                    examnamemap = await _dbContext.PExamStudent.Where(m => m.ExamId == examData.ExamId && m.StudentId == studentData.Id).FirstOrDefaultAsync();
                }

                var facultystudentmap = await _dbContext.PStudentFaculty.Where(m => m.UserId == facultyData.Id && m.ExamStudentId == examnamemap.ExamStudentId).FirstOrDefaultAsync();
                if (facultystudentmap == null)
                {
                    _dbContext.PStudentFaculty.Add(new PStudentFaculty
                    {
                        UserId = facultyData.Id,
                        ExamStudentId = examnamemap.ExamStudentId
                    });
                    await _dbContext.SaveChangesAsync();
                }

            }
            return studentexamfacultyUploadFiles;
        }
    }
}
