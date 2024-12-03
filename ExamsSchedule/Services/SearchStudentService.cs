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

namespace ExamsSchedule.Services
{
    public class SearchStudentService
    {
        private readonly DeepsensorClientContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public string storageAccount_connectionString;
        public IConfiguration _configuration { get; }
        //public string azure_ContainerName;
        public SearchStudentService(DeepsensorClientContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        

        public async Task<List<PEvents>> GetEventbyFiltersAsync(int studentId, int ExamId, string sessionId, string filter = null)
        {
            PExam selectedExam = null;
            if (ExamId > 0)
            {
                selectedExam = await _dbContext.PExam.Where(m => !m.IsDeleted && m.ExamId == ExamId).FirstOrDefaultAsync();
            }

            var record = _dbContext.PEvents.Where(m => m.StudentId == studentId && m.SessionId == sessionId);

            if (ExamId > 0 && selectedExam != null)
            {
                record = record.Where(m => m.ExamIdentifier == selectedExam.ExamIdentifier1);
            }

            if (filter != null)
            {
                record = record.Where(m => m.Event == filter);
            }

            return await record.ToListAsync();
        }


        public async Task<List<StudentImpersonation>> GetImpersonationImages(long Id)
        {
            var selectedExam = await _dbContext.PStudentImpersonation.Where(m => m.ProctoringStatusId == Id).ToListAsync();
            return selectedExam;
        }

        public async Task<List<PEvents>> GetEvents(int studentId, int examId, string sessionId, string filter = null)
        {
            PExam selectedExam = null;
            if (examId > 0)
            {
                //selectedExam = await _dbContext.PExam.Where(m => !m.IsDeleted && m.ExamId == examId).FirstOrDefaultAsync();
                selectedExam = await _dbContext.PExam.Where(m => m.ExamId == examId).FirstOrDefaultAsync();
            }
            // && m.ExamIdentifier == "abc123"
            var record = await _dbContext.PEvents.Where(m => m.StudentId == studentId && m.SessionId == sessionId &&
            (examId > 0 ? m.ExamIdentifier == selectedExam.ExamIdentifier1 : true) && (filter == null ? true : m.Event == filter)).ToListAsync();
            return record;
        }

        public async Task ApproveAsync(long id, int studentId, int examId, int warningType)
        {
            if (warningType == 4) //Yellow
            {
                await SaveMessageStaticAsync(id, "Abberant behavior was detected", 3, 0);
            }

            if (warningType == 3) //Yellow
            {
                await SaveMessageStaticAsync(id, "Please remove the mask", warningType, 0);
            }
            if (warningType == 2) //Orange
            {
                await SaveMessageStaticAsync(id, "Please adjust your camera so that your face is clearly visible", warningType, 0);
            }
            if (warningType == 1) //Red
            {
                await SaveMessageStaticAsync(id, "Please do not allow others to enter exam room", warningType, 0);
            }
            // set all messages to 3 when approve
            if (warningType == 50) //"Approve"
            {
                var data = await _dbContext.LiveProctorMessage.Where(m => m.ProctoringStatusId == id).ToListAsync();
                foreach (var item in data)
                {
                    item.Status = 3;
                }
                await _dbContext.SaveChangesAsync();
            }
        }



        public async Task addExamStudentProctoringStatuslog(long id, string statusValue)
        {
            var userClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier);
            var data = new pExamStudentProctoringStatusLogs
            {
                ProctoringStatuId = id,
                StatusId = statusValue,
                CreatedDate = DateTime.Now,
                CreatedBy = userClaim.Value
            };
            await _dbContext.pExamStudentProctoringStatusLogs.AddAsync(data);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateVideoStatus(string sessionId, bool isLiveVideo)
        {

            var record = await _dbContext.PExamStudentProctoringStatus.Where(m => m.SessionId == sessionId).FirstOrDefaultAsync();
            record.LiveVideo = isLiveVideo;
            if (!isLiveVideo)
            {
                record.VideoStatus = 0;
            }
            await _dbContext.SaveChangesAsync();

        }


        public async Task<PStudent> GetStudentEmail(string imagename)
        {
            var stuEmail = await _dbContext.PStudent.Where(m => m.StudentUniqueId == imagename).FirstOrDefaultAsync();
            return stuEmail;
        }

        public async Task<BlobContainerClient> GetContainer()
        {
            //string storageAccount_connectionString = "DefaultEndpointsProtocol=https;AccountName=proctorimagesus;AccountKey=xv75fUkdnNc1nKHhyJfRwsf09pKC+Bqt34x5YCKJ96btpHq2DG03v+Hv9JrniPVn/gGgpjncdLK5+uKTQpYJCQ==;EndpointSuffix=core.windows.net";

            string storageAccount_connectionString = _configuration["blobConnection"];

            string blobContainerName = "studentidentity";/*_configuration.GetValue<string>("AzureBlobStorage:studentidentity");*/

            var container = new BlobContainerClient(storageAccount_connectionString, blobContainerName);
            await container.CreateIfNotExistsAsync();

            return container;
        }
    
        public async Task<List<PStudent>> GetStudentByUniqueIds(List<string> studentUniqueIds)
        {
            var stuEmail = await _dbContext.PStudent.Where(m => studentUniqueIds.Contains(m.StudentUniqueId)).ToListAsync();
            return stuEmail;
        }

        public async Task<List<BulkStudentImageViewModel>> UploadToBlob(IList<IBrowserFile> imageFile)
        {
            var list = new List<BulkStudentImageViewModel>();

            var fileNames = imageFile.Select(m => Path.GetFileNameWithoutExtension(m.Name)).ToList();
            var students = await GetStudentByUniqueIds(fileNames);

            foreach (var item in imageFile)
            {
                var imageName = item.Name;

                var fileWithoutExtension = Path.GetFileNameWithoutExtension(imageName);

                var studentData = students.Where(m => m.StudentUniqueId == fileWithoutExtension).FirstOrDefault();

                if (studentData != null)
                {
                    var stream = item.OpenReadStream();
                    var container = await GetContainer();

                    var blob = container.GetBlobClient(imageName);
                    await blob.UploadAsync(stream, overwrite: true);

                    studentData.ImageName = imageName;
                    await _dbContext.SaveChangesAsync();
                    list.Add(new BulkStudentImageViewModel
                    {
                        ImageName = imageName,
                        IsSaved = true
                    });
                }
                else
                {
                    list.Add(new BulkStudentImageViewModel
                    {
                        ImageName = imageName,
                        IsSaved = false
                    });
                }

            }
            return list;

        }
       

        public async Task StudentStartedVideo(string sessionId)
        {
            var record = await _dbContext.PExamStudentProctoringStatus.Where(m => m.SessionId == sessionId).FirstOrDefaultAsync();
            if (record != null)
            {
                record.VideoStatus = 1;
                await _dbContext.SaveChangesAsync();
            }

        }


        public async Task UpdateNeedAssitanceStatus(bool needAssitance, long Id)
        {

            var record = await _dbContext.PExamStudentProctoringStatus.Where(m => m.Id == Id).FirstOrDefaultAsync();

            if (record != null)
            {
                record.NeedAssistance = needAssitance;
                _dbContext.Update<PExamStudentProctoringStatus>(record);
                await _dbContext.SaveChangesAsync();
            }


        }

        public async Task StudentStoppedVideo(string sessionId)
        {

            var record = await _dbContext.PExamStudentProctoringStatus.Where(m => m.SessionId == sessionId).FirstOrDefaultAsync();
            if (record != null)
            {
                record.VideoStatus = 2;
                await _dbContext.SaveChangesAsync();
            }

        }
        public async Task<List<PEvents>> GetEventsAsync()
        {
            var events = await _dbContext.PEvents.ToListAsync();
            return events;
        }

        public async Task<List<PExamStudentProctoringStatus>> GetAllDataAsync()
        {
            var data = await _dbContext.PExamStudentProctoringStatus.Include(m => m.StudentId).Include(m => m.ExamId).Where(m => !m.EndedAt.HasValue).ToListAsync();
            return data;
        }

        public async Task<List<pExamStudentProctoringStatusLogs>> GetLogsDataAsync(long id)
        {
            var data = await _dbContext.pExamStudentProctoringStatusLogs.Include("User").Where(m => m.ProctoringStatuId == id).ToListAsync();
            return data;
        }

        public async Task<Lmsintegration> GetlmsDataAsync()
        {
            var data = await _dbContext.Lmsintegration.FirstOrDefaultAsync();
            return data;
        }

        public async Task<Lmsintegration> SaveDataAsync(Lmsintegration model)
        {
            var entity = new Lmsintegration
            {
                WebServiceLink = model.WebServiceLink,
                SecurityToken = model.SecurityToken
            };

            await _dbContext.Lmsintegration.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Lmsintegration> UpdateDataAsync(Lmsintegration model)
        {
            var data = await _dbContext.Lmsintegration.Where(m => m.Id == model.Id).FirstOrDefaultAsync();
            data.WebServiceLink = model.WebServiceLink;
            data.SecurityToken = model.SecurityToken;
            await _dbContext.SaveChangesAsync();
            return data;
        }

        public async Task<LiveProctorMessage> SaveMessageDataAsync(long id, LiveProctorMessage model, int value)
        {
            var userClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier);
            var entity = new LiveProctorMessage
            {
                ProctoringStatusId = id,
                Message = model.Message,
                SendBy = userClaim.Value,
                SendDate = model.SendDate,
                Status = model.Status,
                WarningType = value
            };
            await _dbContext.LiveProctorMessage.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<LiveProctorMessage> SaveMessageStaticAsync(long id, string msg, int value, int status)
        {
            var userClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier);
            var entity = new LiveProctorMessage
            {
                ProctoringStatusId = id,
                Message = msg,
                SendBy = userClaim.Value,
                SendDate = DateTime.UtcNow,
                Status = status,
                WarningType = value
            };
            await _dbContext.LiveProctorMessage.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<List<LiveProctorMessage>> GetMessageLogsDataAsync(long id)
        {
            var data = await _dbContext.LiveProctorMessage.Include("User").Where(m => m.ProctoringStatusId == id).OrderByDescending(m => m.Id).ToListAsync();
            return data;
        }

        public async Task<List<ExamUploads>> GetExamUploads()
        {
            var list = await _dbContext.ExamUploads.OrderByDescending(m => m.CreatedAt).ToListAsync();
            return list;
        }

        public async Task<List<ExamUploads>> ImportData(List<IBrowserFile> imageFile,string filterByType)
        {

            var list = new List<ExamUploads>();

            foreach (var item in imageFile)
            {
                var imageName = item.Name;
                var fileWithoutExtension = Path.GetFileNameWithoutExtension(imageName);
                var fileExtension = Path.GetExtension(imageName);
                var stream = item.OpenReadStream();

                imageName = fileWithoutExtension + "_" + DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss") + fileExtension;

                var container = await GetContainer();
                var blob = container.GetBlobClient(imageName);
                await blob.UploadAsync(stream, overwrite: true);
                list.Add(new ExamUploads
                {
                    FileName = imageName,
                    CreatedAt = DateTime.Now,
                    Status = "In Progress"
                    //Type = filterByType
                });
            }
            return list;
        }

        public async Task<List<ExamUploads>> SaveExamUploadFile(List<ExamUploads> data)
        {
            var list = new List<ExamUploads>();
            foreach (var model in data)
            {
                var entity = new ExamUploads
                {
                    CreatedAt = DateTime.Now,
                    FileName = model.FileName,
                    Status = model.Status
                    //Type = model.Type
                };
                await _dbContext.ExamUploads.AddAsync(entity);
            }
            await _dbContext.SaveChangesAsync();
            return list;

        }

    }
}
