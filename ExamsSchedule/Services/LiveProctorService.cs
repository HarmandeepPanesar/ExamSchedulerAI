using ExamsSchedule.Models.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using TimeZoneConverter;
using System.IO;
using Microsoft.AspNetCore.Components.Forms;
using Azure.Storage.Blobs;
using ExamsSchedule.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace ExamsSchedule.Services
{
    public class LiveProctorService
    {
        private readonly DeepsensorClientContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NavigationManager navigationManager;
        public string storageAccount_connectionString;
        public IConfiguration _configuration { get; }
        //public string azure_ContainerName;
        public LiveProctorService(DeepsensorClientContext dbContext,
            NavigationManager navigationManager,
            IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            this.navigationManager = navigationManager;
        }

        public void Export(string type, int skip = 0, int take = 0,
            string txtSearchStudent = null, string txtSearchExam = null,
            string proctorSearch = null, DateTime? pastDate = null, DateTime? futureDate = null,
            int rdoFilterLiveExam = 0)
        {
            navigationManager.NavigateTo($"/export/northwind?type={type}&skip={skip}&take={take}" +
                $"&txtSearchStudent={txtSearchStudent}&txtSearchExam={txtSearchExam}&proctorSearch={proctorSearch}&pastDate={pastDate}" +
                $"&futureDate={futureDate}&rdoFilterLiveExam={rdoFilterLiveExam}", true);
        }

        public async Task<List<PExamStudentProctoringStatus>> GetDataAsync(int studentId, int ExamId, string filterByDays, DateTime? selectedFromDate,DateTime? selectedToDate,bool activeOnly = false)
        {

            var result = new List<PExamStudentProctoringStatus>();

            var disable = true; //  TODO: Make this a user setting.

            if (!disable && _httpContextAccessor.HttpContext.User.IsInRole("User"))
            {
                //get proctor Id
                var userClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier);

                //get all exams proctor deals in
                var assignedStudents = await _dbContext.PStudentFaculty.Where(m => m.UserId == userClaim.Value).Select(m => m.ExamStudent).AsNoTracking().ToListAsync();
                var aslist = assignedStudents.ToList();

                var examIds = assignedStudents.Select(m => m.ExamId).ToArray();
                var studentIds = assignedStudents.Select(m => m.StudentId).ToArray();

                if (activeOnly)
                {
                    var resulttemp1 = await _dbContext.PExamStudentProctoringStatus.Include("Exam").Include("Student")
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

                    result = k;
                }
                else
                {
                    var resulttemp1 = await _dbContext.PExamStudentProctoringStatus.Include("Exam").Include("Student")
                       .Where(m => studentIds.Contains(m.StudentId) && examIds.Contains(m.ExamId)).AsNoTracking().ToListAsync();

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

                    result = k;
                }

            }
            else
            {
                if (activeOnly)
                {
                    //result = await _dbContext.PExamStudentProctoringStatus.Include("Exam").Include("Student").Include("PProctroingEvaluation").Where(m => m.EndedAt == null).AsNoTracking().ToListAsync();
                    result = await _dbContext.PExamStudentProctoringStatus.Include("Student").Include("Exam").Where(m => m.EndedAt == null).AsNoTracking().ToListAsync();
                }
                else
                {
                    result = await _dbContext.PExamStudentProctoringStatus.Include("Exam").Include("Student").AsNoTracking().ToListAsync();
                }
            }

            if (filterByDays != null)
            {
                if (filterByDays == "Last 5 Days")
                {
                    var filterDate = DateTime.Now.AddDays(-5).Date;
                    result = result.Where(m => m.StartedAt.HasValue && m.StartedAt.Value.Date > filterDate).ToList();
                }
                else if (filterByDays == "Last 30 Days")
                {
                    var filterDate = DateTime.Now.AddDays(-30).Date;
                    result = result.Where(m => m.StartedAt.HasValue && m.StartedAt.Value.Date > filterDate).ToList();
                }
                else if (filterByDays == "Last 90 Days")
                {
                    var filterDate = DateTime.Now.AddDays(-90).Date;
                    result = result.Where(m => m.StartedAt.HasValue && m.StartedAt.Value.Date > filterDate).ToList();
                }

            }

            //var events = await _dbContext.PEvents.ToListAsync();
            PExam selectedExam = null;
            if (studentId > 0)
            {
                result = result.Where(m => m.StudentId == studentId).ToList();
            }

            if (ExamId > 0)
            {
                result = result.Where(m => m.ExamId == ExamId).ToList();
                selectedExam = await _dbContext.PExam.Where(m => !m.IsDeleted && m.ExamId == ExamId).FirstOrDefaultAsync();
            }
            //check if msg is available
            var pStatusId = result.Select(m => m.Id).ToList();
            // var message = await _dbContext.LiveProctorMessage.Where(m => pStatusId.Contains(m.ProctoringStatusId)).Select(m => m.ProctoringStatusId).Distinct().ToListAsync();
            var message = await _dbContext.LiveProctorMessage.AsNoTracking().Where(m => pStatusId.Contains(m.ProctoringStatusId)).ToListAsync();
            var enableChatData = await _dbContext.Lmsintegration.AsNoTracking().FirstOrDefaultAsync();
            foreach (var item in result)
            {
                item.IsMessageAvaiable = message.Where(m => m.ProctoringStatusId == item.Id).Any();
                var msg = message.Where(m => m.ProctoringStatusId == item.Id).OrderByDescending(m => m.SendDate).FirstOrDefault();
                if (msg != null)
                {
                    item.MessageStatus = msg.Status;
                }
                if (enableChatData != null)
                {
                    item.EnableChat = enableChatData.EnableChat;
                }
            }
            if (selectedToDate != null && selectedFromDate != null)
            {
                result = result.Where(m => m.StartedAt.Value.Date >= selectedFromDate.Value.Date && m.EndedAt.Value.Date <= selectedToDate.Value.Date).ToList();
            }
            else if ( selectedFromDate != null && selectedToDate == null)
            {
                result = result.Where(m => m.StartedAt.Value.Date == selectedFromDate.Value.Date).ToList();
            }
            else
            {
                return result;
            }
            return result;
        }

        public async Task<List<pLogs>> GetReportsLogDataAsync(string filterByDays)
        {
            var result = _dbContext.pLogs.Include(m => m.Student).Include(m => m.Exam).AsQueryable();
            if (filterByDays != null)
            {
                if (filterByDays == "Last 5 Days")
                {
                    var filterDate = DateTime.Now.AddDays(-5).Date;
                    result = result.Where(m => m.CreatedAt.HasValue && m.CreatedAt.Value.Date > filterDate);
                }
                else if (filterByDays == "Last 30 Days")
                {
                    var filterDate = DateTime.Now.AddDays(-30).Date;
                    result = result.Where(m => m.CreatedAt.HasValue && m.CreatedAt.Value.Date > filterDate);
                }
                else if (filterByDays == "Last 90 Days")
                {
                    var filterDate = DateTime.Now.AddDays(-90).Date;
                    result = result.Where(m => m.CreatedAt.HasValue && m.CreatedAt.Value.Date > filterDate);
                }

            }
            return await result.ToListAsync();

        }

        public async Task<int> GetMessageReportDataCountAsync(string txtSearchStudent = null, string txtSearchExam = null,
            string proctorSearch = null, DateTime? pastDate = null, DateTime? futureDate = null,
            int rdoFilterLiveExam = 0)
        {
            var students = _dbContext.LiveProctorMessage.Include(m => m.User).Include(m => m.ProctoringStatus.Exam).Include(m => m.ProctoringStatus.Student).OrderByDescending(m => m.Id).AsQueryable();

            if (rdoFilterLiveExam == 1) //live only
            {
                students = students.Where(m => m.ProctoringStatus.EndedAt != null);
            }
            else if (rdoFilterLiveExam == 2) //non live only
            {
                students = students.Where(m => m.ProctoringStatus.EndedAt == null);
            }

            if (txtSearchStudent != null)
            {
                students = students.Where(m => (m.ProctoringStatus.Student.FirstName + " " + m.ProctoringStatus.Student.MiddleName + " " + m.ProctoringStatus.Student.LastName).Contains(txtSearchStudent)
            || (m.ProctoringStatus.Student.FirstName + " " + m.ProctoringStatus.Student.MiddleName).Contains(txtSearchStudent)
            || (m.ProctoringStatus.Student.FirstName + " " + m.ProctoringStatus.Student.LastName).Contains(txtSearchStudent)
            || (m.ProctoringStatus.Student.MiddleName + " " + m.ProctoringStatus.Student.LastName).Contains(txtSearchStudent)
            || (!string.IsNullOrEmpty(m.ProctoringStatus.Student.FirstName) && m.ProctoringStatus.Student.FirstName.Contains(txtSearchStudent))
            || (!string.IsNullOrEmpty(m.ProctoringStatus.Student.MiddleName) && m.ProctoringStatus.Student.MiddleName.Contains(txtSearchStudent))
            || (!string.IsNullOrEmpty(m.ProctoringStatus.Student.LastName) && m.ProctoringStatus.Student.LastName.Contains(txtSearchStudent))
            || (!string.IsNullOrEmpty(m.ProctoringStatus.Student.Email) && m.ProctoringStatus.Student.Email.Contains(txtSearchStudent))
            || (!string.IsNullOrEmpty(m.ProctoringStatus.Student.StudentUniqueId) && m.ProctoringStatus.Student.StudentUniqueId.Contains(txtSearchStudent))
            || (!string.IsNullOrEmpty(m.ProctoringStatus.Student.StudentIdentity) &&
                m.ProctoringStatus.Student.StudentIdentity.Contains(txtSearchStudent)));
            }
            if (txtSearchExam != null)
            {
                students = students.Where(m => !m.ProctoringStatus.Exam.IsDeleted &&
                (m.ProctoringStatus.Exam.ExameName.Contains(txtSearchExam)
                || (!string.IsNullOrEmpty(m.ProctoringStatus.Exam.ExamDescription) && m.ProctoringStatus.Exam.ExamDescription.Contains(txtSearchExam))
                || (!string.IsNullOrEmpty(m.ProctoringStatus.Exam.ExamIdentifier1) && m.ProctoringStatus.Exam.ExamIdentifier1.Contains(txtSearchExam))
                || (!string.IsNullOrEmpty(m.ProctoringStatus.Exam.ExamIdentifier2) && m.ProctoringStatus.Exam.ExamIdentifier2.Contains(txtSearchExam))));
            }
            if (proctorSearch != null)
            {
                students = students.Where(m => (m.User.UserName.Contains(proctorSearch)
            || (!string.IsNullOrEmpty(m.User.Email) && m.User.Email.Contains(proctorSearch))
            || (!string.IsNullOrEmpty(m.User.UserName) && m.User.UserName.Contains(proctorSearch))));

            }
            if (pastDate != null && futureDate != null)
            {
                students = students.Where(m => m.SendDate >= pastDate && m.SendDate <= futureDate);
            }

            return await students.CountAsync();
        }

        public async Task<List<LiveProctorMessage>> GetMessageReportsDataAsync(int skip = 0, int take = 0,
            string txtSearchStudent = null, string txtSearchExam = null,
            string proctorSearch = null, DateTime? pastDate = null, DateTime? futureDate = null,
            int rdoFilterLiveExam = 0,DateTime? startDate = null,DateTime? endDate=null)
        {
            var students = _dbContext.LiveProctorMessage.Include(m => m.User).Include(m => m.ProctoringStatus.Exam).Include(m => m.ProctoringStatus.Student).OrderByDescending(m => m.Id).AsQueryable();

            if (rdoFilterLiveExam == 1) //live only
            {
                students = students.Where(m => m.ProctoringStatus.EndedAt != null);
            }
            else if (rdoFilterLiveExam == 2) //non live only
            {
                students = students.Where(m => m.ProctoringStatus.EndedAt == null);
            }

            if (txtSearchStudent != null)
            {
                students = students.Where(m => (m.ProctoringStatus.Student.FirstName + " " + m.ProctoringStatus.Student.MiddleName + " " + m.ProctoringStatus.Student.LastName).Contains(txtSearchStudent)
            || (m.ProctoringStatus.Student.FirstName + " " + m.ProctoringStatus.Student.MiddleName).Contains(txtSearchStudent)
            || (m.ProctoringStatus.Student.FirstName + " " + m.ProctoringStatus.Student.LastName).Contains(txtSearchStudent)
            || (m.ProctoringStatus.Student.MiddleName + " " + m.ProctoringStatus.Student.LastName).Contains(txtSearchStudent)
            || (!string.IsNullOrEmpty(m.ProctoringStatus.Student.FirstName) && m.ProctoringStatus.Student.FirstName.Contains(txtSearchStudent))
            || (!string.IsNullOrEmpty(m.ProctoringStatus.Student.MiddleName) && m.ProctoringStatus.Student.MiddleName.Contains(txtSearchStudent))
            || (!string.IsNullOrEmpty(m.ProctoringStatus.Student.LastName) && m.ProctoringStatus.Student.LastName.Contains(txtSearchStudent))
            || (!string.IsNullOrEmpty(m.ProctoringStatus.Student.Email) && m.ProctoringStatus.Student.Email.Contains(txtSearchStudent))
            || (!string.IsNullOrEmpty(m.ProctoringStatus.Student.StudentUniqueId) && m.ProctoringStatus.Student.StudentUniqueId.Contains(txtSearchStudent))
            || (!string.IsNullOrEmpty(m.ProctoringStatus.Student.StudentIdentity) &&
                m.ProctoringStatus.Student.StudentIdentity.Contains(txtSearchStudent)));
            }
            if (txtSearchExam != null)
            {
                students = students.Where(m => !m.ProctoringStatus.Exam.IsDeleted &&
                (m.ProctoringStatus.Exam.ExameName.Contains(txtSearchExam)
                || (!string.IsNullOrEmpty(m.ProctoringStatus.Exam.ExamDescription) && m.ProctoringStatus.Exam.ExamDescription.Contains(txtSearchExam))
                || (!string.IsNullOrEmpty(m.ProctoringStatus.Exam.ExamIdentifier1) && m.ProctoringStatus.Exam.ExamIdentifier1.Contains(txtSearchExam))
                || (!string.IsNullOrEmpty(m.ProctoringStatus.Exam.ExamIdentifier2) && m.ProctoringStatus.Exam.ExamIdentifier2.Contains(txtSearchExam))));
            }
            if (proctorSearch != null)
            {
                students = students.Where(m => (m.User.UserName.Contains(proctorSearch)
            || (!string.IsNullOrEmpty(m.User.Email) && m.User.Email.Contains(proctorSearch))
            || (!string.IsNullOrEmpty(m.User.UserName) && m.User.UserName.Contains(proctorSearch))));

            }
            if (pastDate != null && futureDate != null)
            {
                students = students.Where(m => m.SendDate >= pastDate && m.SendDate <= futureDate);
            }

            if (take > 0)
            {
                students = students.Skip(skip).Take(take);
            }
            if (startDate != null && endDate != null)
            {
                students = students.Where(m => m.SendDate.Date >= startDate.Value.Date && m.SendDate.Date <= endDate.Value.Date);
            }
            else if (startDate != null && endDate == null)
            {
                students = students.Where(m => m.SendDate.Date == startDate.Value.Date);
            }
            else
            {
                return await students.ToListAsync();
            }
            return await students.ToListAsync();
        }

    }
}
