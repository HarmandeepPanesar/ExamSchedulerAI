using ExamsSchedule.Data;
using ExamsSchedule.Models.DB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Services
{
    public class ExamStatusService
    {
        private readonly DeepsensorClientContext _dbContext;

        public ExamStatusService(DeepsensorClientContext dbContext)
        {
            _dbContext = dbContext;
        }

        //to get all the data
        public async Task<List<PExamStudentProctoringStatus>> GetAllAsync(System.Linq.Expressions.Expression<Func<PExamStudentProctoringStatus, bool>> predict = null)
        {
            var exams = await _dbContext.PExamStudentProctoringStatus.Where(predict).ToListAsync();
            return exams;
        }
        
    }
}
