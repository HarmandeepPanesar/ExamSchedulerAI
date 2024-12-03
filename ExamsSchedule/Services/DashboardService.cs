using ExamsSchedule.Models.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamsSchedule.Services
{
    public class DashboardService
    {
        private readonly DeepsensorClientContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardService(DeepsensorClientContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }
        public async Task<List<DashboardItems>> GetAllItems()
        {
            var dashboardItems = new List<DashboardItems>();
            if (_httpContextAccessor.HttpContext.User.IsInRole("User"))
            {
                dashboardItems = await _dbContext.DashboardItems.Where(m=>m.Role == "User").ToListAsync();
            }
            else
            {
                dashboardItems = await _dbContext.DashboardItems.Where(m => m.Role == "Admin").ToListAsync();
            }
            return dashboardItems;
        }

    }
}
