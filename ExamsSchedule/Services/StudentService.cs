using Azure.Storage.Blobs;
using ExamsSchedule.Data;
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
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;


namespace ExamsSchedule.Services
{
    public class StudentService
    {
        private readonly DeepsensorClientContext _dbContext;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IConfiguration _configuration;

        public StudentService(DeepsensorClientContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public string GetUserId()
        {
            var userClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier);
            return userClaim.Value;
        }

        public async Task<int> GetAllDataCountAsync(string filter = null)
        {
            var userIdentity = _httpContextAccessor.HttpContext.User;

            if (userIdentity.IsInRole("Admin"))
            {
                var students = _dbContext.PStudent.Include("PExamStudent.Exam").Where(m => !m.IsDeleted);

                if (filter != null)
                {
                    students = students.Where(m => (m.FirstName + " " + m.MiddleName + " " + m.LastName).Contains(filter)
                    || (m.FirstName + " " + m.MiddleName).Contains(filter)
                    || (m.FirstName + " " + m.LastName).Contains(filter)
                    || (m.MiddleName + " " + m.LastName).Contains(filter)
                    || (!string.IsNullOrEmpty(m.FirstName) && m.FirstName.Contains(filter))
                    || (!string.IsNullOrEmpty(m.MiddleName) && m.MiddleName.Contains(filter))
                    || (!string.IsNullOrEmpty(m.LastName) && m.LastName.Contains(filter))
                    || (!string.IsNullOrEmpty(m.Email) && m.Email.Contains(filter))
                    || (!string.IsNullOrEmpty(m.StudentUniqueId) && m.StudentUniqueId.Contains(filter))
                    || (!string.IsNullOrEmpty(m.StudentIdentity) && m.StudentIdentity.Contains(filter)));
                }

                return await students.CountAsync();
            }
            else
            {
                var loggedInUserId = GetUserId();
                var assignedStudents = await _dbContext.PStudentFaculty.Where(m => m.UserId == loggedInUserId).Select(m => m.ExamStudent.Student.Id).ToListAsync();

                var students = _dbContext.PStudent.Include("PExamStudent.Exam").Where(m => !m.IsDeleted && assignedStudents.Contains(m.Id));

                if (filter != null)
                {
                    students = students.Where(m => (m.FirstName + " " + m.MiddleName + " " + m.LastName).Contains(filter)
                    || (m.FirstName + " " + m.MiddleName).Contains(filter)
                    || (m.FirstName + " " + m.LastName).Contains(filter)
                    || (m.MiddleName + " " + m.LastName).Contains(filter)
                    || (!string.IsNullOrEmpty(m.FirstName) && m.FirstName.Contains(filter))
                    || (!string.IsNullOrEmpty(m.MiddleName) && m.MiddleName.Contains(filter))
                    || (!string.IsNullOrEmpty(m.LastName) && m.LastName.Contains(filter))
                    || (!string.IsNullOrEmpty(m.Email) && m.Email.Contains(filter))
                    || (!string.IsNullOrEmpty(m.StudentUniqueId) && m.StudentUniqueId.Contains(filter))
                    || (!string.IsNullOrEmpty(m.StudentIdentity) && m.StudentIdentity.Contains(filter)));
                }

                return await students.CountAsync();
            }
        }

        public async Task<List<PStudent>> GetAllDataAsync(int skip = 0, int take = 0, string filter = null)
        {
            //do not remove comment
            //update in GetAllDataCountAsync as well
            var userIdentity = _httpContextAccessor.HttpContext.User;

            if (userIdentity.IsInRole("Admin"))
            {
                var students = _dbContext.PStudent.Include("PExamStudent.Exam").Where(m => !m.IsDeleted);

                if (filter != null)
                {
                    students = students.Where(m => (m.FirstName + " " + m.MiddleName + " " + m.LastName).Contains(filter)
                    || (m.FirstName + " " + m.MiddleName).Contains(filter)
                    || (m.FirstName + " " + m.LastName).Contains(filter)
                    || (m.MiddleName + " " + m.LastName).Contains(filter)
                    || (!string.IsNullOrEmpty(m.FirstName) && m.FirstName.Contains(filter))
                    || (!string.IsNullOrEmpty(m.MiddleName) && m.MiddleName.Contains(filter))
                    || (!string.IsNullOrEmpty(m.LastName) && m.LastName.Contains(filter))
                    || (!string.IsNullOrEmpty(m.Email) && m.Email.Contains(filter))
                    || (!string.IsNullOrEmpty(m.StudentUniqueId) && m.StudentUniqueId.Contains(filter))
                    || (!string.IsNullOrEmpty(m.StudentIdentity) && m.StudentIdentity.Contains(filter)));
                }

                if (take > 0)
                {
                    students = students.Skip(skip).Take(take);
                }

                return await students.ToListAsync();
            }
            else
            {
                var loggedInUserId = GetUserId();
                var assignedStudents = await _dbContext.PStudentFaculty.Where(m => m.UserId == loggedInUserId).Select(m => m.ExamStudent.Student.Id).ToListAsync();

                var students = _dbContext.PStudent.Include("PExamStudent.Exam").Where(m => !m.IsDeleted && assignedStudents.Contains(m.Id));

                if (filter != null)
                {
                    students = students.Where(m => (m.FirstName + " " + m.MiddleName + " " + m.LastName).Contains(filter)
                    || (m.FirstName + " " + m.MiddleName).Contains(filter)
                    || (m.FirstName + " " + m.LastName).Contains(filter)
                    || (m.MiddleName + " " + m.LastName).Contains(filter)
                    || (!string.IsNullOrEmpty(m.FirstName) && m.FirstName.Contains(filter))
                    || (!string.IsNullOrEmpty(m.MiddleName) && m.MiddleName.Contains(filter))
                    || (!string.IsNullOrEmpty(m.LastName) && m.LastName.Contains(filter))
                    || (!string.IsNullOrEmpty(m.Email) && m.Email.Contains(filter))
                    || (!string.IsNullOrEmpty(m.StudentUniqueId) && m.StudentUniqueId.Contains(filter))
                    || (!string.IsNullOrEmpty(m.StudentIdentity) && m.StudentIdentity.Contains(filter)));
                }

                if (take > 0)
                {
                    students = students.Skip(skip).Take(take);
                }

                return await students.ToListAsync();
            }
        }

        public async Task<List<PStudent>> SearchAssignStudentAsync(int examId, string filter = null)
        {
            var studList = new List<PStudent>();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                studList = await _dbContext.PExamStudent.Include("Student").Where(m => m.ExamId == examId && (m.Student.FirstName + " " + m.Student.LastName).Contains(filter)).Select(m => m.Student).ToListAsync();
            }
            return studList;
        }
        public async Task<List<PStudent>> SearchStudentAsync(string filter = null)
        {
            var studList = new List<PStudent>();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                studList = await _dbContext.PExamStudent.Include("Student").Where(m => (m.Student.FirstName + " " + m.Student.LastName).Contains(filter)).Select(m => m.Student).ToListAsync();
            }
            return studList;
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

        public async Task<PStudent> AddStudentAsync(PStudent model, IList<IBrowserFile> imageFile)
        {
            var loggedInUserId = GetUserId();
            foreach (var item in imageFile)
            {
                var entity = new PStudent
                {
                    FirstName = model.FirstName,
                    MiddleName = model.MiddleName,
                    LastName = model.LastName,
                    DoB = model.DoB,
                    Email = model.Email,
                    StudentIdentity = model.StudentIdentity,
                    StudentUniqueId = model.StudentUniqueId,
                    Active = model.Active,
                    ImageName = item.Name,
                    CreatedDate = DateTime.Now,
                    CreatedBy = loggedInUserId
                };
                await _dbContext.PStudent.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                //assign ro faculy if faculy login
                var userIdentity = _httpContextAccessor.HttpContext.User;
                if (userIdentity.IsInRole("User"))
                {
                    var user = await _userManager.FindByNameAsync(userIdentity.Identity.Name);
                    await new StudentFacultyService(_dbContext).AddstudentfacultyAsync(user, new List<int> { entity.Id }, false);
                }
            }
            var fileNames = imageFile.Select(m => Path.GetFileNameWithoutExtension(m.Name)).ToList();

            foreach (var item in imageFile)
            {
                var imageName = item.Name;

                var stream = item.OpenReadStream();
                var container = await GetContainer();

                var blob = container.GetBlobClient(imageName);
                await blob.UploadAsync(stream, overwrite: true);
            }
            var entity1 = await _dbContext.PStudent.Where(m => m.Id == model.Id).FirstOrDefaultAsync();
            return entity1;
        }

        public async Task BulkUploads(StudentJson[] jsonStudent)
        {
            var loggedInUserId = GetUserId();
            foreach (var item in jsonStudent)
            {
                var studentUId = item.id.ToString();
                var data = await _dbContext.PStudent.Where(m => (m.StudentIdentity == studentUId || m.StudentUniqueId == studentUId)).FirstOrDefaultAsync();
                if (data != null)
                {
                    data.FirstName = item.firstname;
                    data.LastName = item.lastname;
                    data.Email = item.email;
                    data.StudentIdentity = studentUId;
                    data.StudentUniqueId = studentUId;
                    data.IsDeleted = false;
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    var entity = new PStudent();
                    entity.FirstName = item.firstname;
                    entity.LastName = item.lastname;
                    entity.Email = item.email;
                    entity.Active = true;
                    entity.StudentIdentity = studentUId;
                    entity.StudentUniqueId = studentUId;
                    entity.CreatedDate = DateTime.Now;
                    entity.CreatedBy = loggedInUserId;
                    await _dbContext.PStudent.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<PStudent> UpdateStudentAsync(PStudent model, IList<IBrowserFile> imageFile)
        {
            var data = await _dbContext.PStudent.Where(m => !m.IsDeleted && m.Id == model.Id).FirstOrDefaultAsync();
            if (imageFile.Count() == 0)
            {
                data.FirstName = model.FirstName;
                data.MiddleName = model.MiddleName;
                data.LastName = model.LastName;
                data.DoB = model.DoB;
                data.Email = model.Email;
                data.Active = model.Active;
                //data.ImageName = item.Name;
                data.StudentIdentity = model.StudentIdentity;
                data.StudentUniqueId = model.StudentUniqueId;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var studentImageFile = imageFile.FirstOrDefault();
                if (data != null)
                {
                    data.FirstName = model.FirstName;
                    data.MiddleName = model.MiddleName;
                    data.LastName = model.LastName;
                    data.DoB = model.DoB;
                    data.Email = model.Email;
                    data.Active = model.Active;
                    data.ImageName = studentImageFile.Name;
                    data.StudentIdentity = model.StudentIdentity;
                    data.StudentUniqueId = model.StudentUniqueId;
                    await _dbContext.SaveChangesAsync();
                }

                var imageName = studentImageFile.Name;
                var stream = studentImageFile.OpenReadStream();
                var container = await GetContainer();
                var blob = container.GetBlobClient(imageName);
                await blob.UploadAsync(stream, overwrite: true);
            }

            return data;
        }

        public async Task DeleteStudent(PStudent model)
        {
            var record = await _dbContext.PStudent.Where(m => !m.IsDeleted && m.Id == model.Id).FirstOrDefaultAsync();
            record.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
        }
    }
}
