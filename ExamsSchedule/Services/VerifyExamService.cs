using ExamsSchedule.Models.DB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Services
{
    public class VerifyExamService
    {
        private readonly DeepsensorClientContext _dbContext;

        public VerifyExamService(DeepsensorClientContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PExamStudentProctoringStatus>> GetAllDataAsync(int ExamId, int StudentId, int StatusId)
        {
            var exams = await _dbContext.PExamStudentProctoringStatus.Where(m => m.ExamId == ExamId && m.StudentId == StudentId && m.ExamStatus == StatusId).ToListAsync();
            return exams;
        }

        public async Task<List<PEvents>> GetEvents(int StudentId, string Identifier)
        {
            var events = await _dbContext.PEvents.Where(m => m.StudentId == StudentId && m.ExamIdentifier == Identifier).ToListAsync();
            return events;
        }

        public async Task<PExamStudentProctoringStatus> GetStudentProctoring(int id)
        {
            var events = await _dbContext.PExamStudentProctoringStatus.Include(m => m.Student).Include(m => m.Exam).Where(m => m.Id == id).FirstOrDefaultAsync();
            return events;
        }
    }
}
