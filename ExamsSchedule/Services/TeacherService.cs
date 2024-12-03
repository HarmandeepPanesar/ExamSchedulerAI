using ExamsSchedule.Models.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExamsSchedule.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ExamsSchedule.Services
{
    public class TeacherService
    {
        private readonly DeepsensorClientContext _dbContext;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private IPasswordHasher<IdentityUser> _passwordHasher;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TeacherService(DeepsensorClientContext dbContext, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, IPasswordHasher<IdentityUser> passwordHash)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _passwordHasher = passwordHash;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<IdentityUser>> GetAllDataAsync(string filter = null)
        {
            var users = await _userManager.GetUsersInRoleAsync("User");
            var usersList = new List<IdentityUser>();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = filter.ToLower();
                usersList = users.Where(m => m.Email.ToLower().Contains(filter)).ToList();
            }
            else
            {
                usersList = users.ToList();
            }
            return usersList;
        }
        
        public async Task<IdentityUser> GetUserAsync(string filter)
        {
            if (!string.IsNullOrEmpty(filter))
                filter = filter.ToLower();
            return await _userManager.Users.Where(m => m.Email.ToLower().Contains(filter)).FirstOrDefaultAsync();
        }

        public async Task<IdentityResult> AddUserAsync(RegisterViewModel model)
        {
            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                //get user to assign role
                var identityUser = await _userManager.FindByNameAsync(model.Email);
                await _userManager.AddToRoleAsync(identityUser, "User");
            }
            return result;
        }

        public async Task<IdentityResult> UpdateUser(RegisterViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (!string.IsNullOrEmpty(model.Password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
            }
            var result = await _userManager.UpdateAsync(user);
            return result;
        }

        public async Task<IdentityResult> DeleteTeacher(IdentityUser model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            var result = await _userManager.DeleteAsync(user);
            return result;
        }

        public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var userIdentity = _httpContextAccessor.HttpContext.User;

            var user = await _userManager.GetUserAsync(userIdentity);

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            //if (changePasswordResult.Succeeded)
            //{
            //    await _signInManager.RefreshSignInAsync(user);
            //}
            return changePasswordResult;
        }
    }
}
