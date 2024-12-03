using ExamsSchedule.Models.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExamsSchedule.Services
{
    public class LiveViewService
    {
        private readonly DeepsensorClientContext db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public string storageAccount_connectionString;
        public IConfiguration _configuration { get; }
        //public string azure_ContainerName;
        public LiveViewService(DeepsensorClientContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            db = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public async Task<List<PExamStudentProctoringStatus>> GetAllDataAsync()
        {
            var result = new List<PExamStudentProctoringStatus>();

            if (_httpContextAccessor.HttpContext.User.IsInRole("User"))
            {
                //get proctor Id
                //var userClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier);

                ////get all exams proctor deals in
                //var assignedStudents = await db.PStudentFaculty.Where(m => m.UserId == userClaim.Value).Select(m => m.ExamStudent).AsNoTracking().ToListAsync();
                //var examIds = assignedStudents.Select(m => m.ExamId).ToArray();
                //var studentIds = assignedStudents.Select(m => m.StudentId).ToArray();

                //result = await db.PExamStudentProctoringStatus.Include("Student")
                //    .Where(m => m.EndedAt == null && studentIds.Contains(m.StudentId) && examIds.Contains(m.ExamId)).AsNoTracking().ToListAsync();

                //get proctor Id
                var userClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier);

                //get all exams proctor deals in
                var assignedStudents = await db.PStudentFaculty.Where(m => m.UserId == userClaim.Value).Select(m => m.ExamStudent).AsNoTracking().ToListAsync();
                var aslist = assignedStudents.ToList();

                var examIds = assignedStudents.Select(m => m.ExamId).ToArray();
                var studentIds = assignedStudents.Select(m => m.StudentId).ToArray();
                var resulttemp1 = await db.PExamStudentProctoringStatus.Include("Exam").Include("Student")
                     .Where(m => m.EndedAt == null && studentIds.Contains(m.StudentId) && examIds.Contains(m.ExamId)).AsNoTracking().ToListAsync();

                var resulttemp2 = from temp in resulttemp1
                                  join a in aslist
                           on new { temp.StudentId, temp.ExamId }
                           equals new { a.StudentId, a.ExamId }
                                  select temp;


                List<PExamStudentProctoringStatus> k = new List<PExamStudentProctoringStatus>();
                foreach (var t in resulttemp2)
                {
                    k.Add(t);
                }

                return k;
            }
            else
            {
                result = await db.PExamStudentProctoringStatus.Include("Student").Include("Exam").Where(m => m.EndedAt == null).AsNoTracking().ToListAsync();
            }

            return result;
        }
    }
}
