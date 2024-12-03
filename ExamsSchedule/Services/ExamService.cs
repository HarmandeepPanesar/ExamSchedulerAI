using ExamsSchedule.Data;
using ExamsSchedule.Models.DB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Services
{
    public class ExamService
    {
        private readonly DeepsensorClientContext _dbContext;

        public ExamService(DeepsensorClientContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> GetAllExamsCount(int skip = 0, int take = 0, string filter = null)
        {
            IQueryable<PExam> exams = ExamWithFilters(skip, take, filter);

            return await exams.CountAsync();
        }

        private IQueryable<PExam> ExamWithFilters(int skip, int take, string filter)
        {
            var exams = _dbContext.PExam.Where(m => !m.IsDeleted);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                exams = exams.Where(m => !m.IsDeleted &&
                (m.ExameName.Contains(filter)
                || (!string.IsNullOrEmpty(m.ExamDescription) && m.ExamDescription.Contains(filter))
                || (!string.IsNullOrEmpty(m.ExamIdentifier1) && m.ExamIdentifier1.Contains(filter))
                || (!string.IsNullOrEmpty(m.ExamIdentifier2) && m.ExamIdentifier2.Contains(filter))));
            }

            if (take > 0)
            {
                exams = exams.Skip(skip).Take(take);
            }

            return exams;
        }

        //to get all the data
        public async Task<List<PExam>> GetAllDataAsync(int skip = 0, int take = 0, string filter = null)
        {
            IQueryable<PExam> exams = ExamWithFilters(skip, take, filter);
            //var exams = await _dbContext.PExam.Include("ExamStudents").ToListAsync();
            return await exams.ToListAsync();
            //return exams;
        }

        //to get all the data
        public async Task<List<PProctoringEnum>> GetAllStatusAsync()
        {
            var status = await _dbContext.PProctoringEnum.ToListAsync();
            //var exams = await _dbContext.PExam.Include("ExamStudents").ToListAsync();
            return status;
        }

        /// <summary>
        /// to add new record
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<PExam> AddExamAsync(PExam model)
        {
            var entity = new PExam
            {
                ExameName = model.ExameName,
                ExamDescription = model.ExamDescription,
                ExamIdentifier1 = model.ExamIdentifier1,
                ExamIdentifier2 = model.ExamIdentifier2,
                Active = model.Active,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };
            await _dbContext.PExam.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }


        public async Task<PExam> UpdateExamAsync(PExam model)
        {
            var data = await _dbContext.PExam.Where(m => !m.IsDeleted && m.ExamId == model.ExamId).FirstOrDefaultAsync();
            data.ExameName = model.ExameName;
            data.ExamDescription = model.ExamDescription;
            data.ExamIdentifier1 = model.ExamIdentifier1;
            data.ExamIdentifier2 = model.ExamIdentifier2;
            data.Active = model.Active;
            data.StartDate = model.StartDate;
            data.EndDate = model.EndDate;
            await _dbContext.SaveChangesAsync();
            return data;
        }

        public async Task DeleteExam(PExam model)
        {
            var record = await _dbContext.PExam.Where(m => !m.IsDeleted && m.ExamId == model.ExamId).FirstOrDefaultAsync();
            record.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
        }


        public async Task<PExam> BulkUploadExams(ExamUploadModel examJson)
        {
            //var userClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.NameIdentifier);
            var cmid = examJson.cm.id.ToString();
            var data = await _dbContext.PExam.Where(m => !m.IsDeleted && m.ExamIdentifier1 == cmid || m.ExamIdentifier2 == cmid).FirstOrDefaultAsync();
            if (data != null)
            {
                //update

                data.ExameName = examJson.cm.name;
                data.ExamDescription = examJson.cm.name;
                data.Active = true;
                data.ExamIdentifier1 = cmid;
                data.ExamIdentifier2 = cmid;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                //add

                var entity = new PExam();

                entity.ExameName = examJson.cm.name;
                entity.ExamDescription = examJson.cm.name;
                entity.Active = true;
                entity.ExamIdentifier1 = cmid;
                entity.ExamIdentifier2 = cmid;
                entity.StartDate = DateTime.Now;
                entity.EndDate = DateTime.Now.AddDays(100);
                await _dbContext.PExam.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
            }

            return data;

        }
    }
}
