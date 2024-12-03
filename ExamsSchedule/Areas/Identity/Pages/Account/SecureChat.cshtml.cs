using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ExamsSchedule.Services;

namespace ExamsSchedule.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class SecureChatModel : PageModel
    {
        public string ReturnUrl { get; set; }
        SearchStudentService searchStudentService;

        [BindProperty]
        public string SessionId { get; set; }

        public SecureChatModel(SearchStudentService svc)
        {
            searchStudentService = svc;
        }
        public async Task OnGetAsync(string vurl = null, bool pclosing = false)
        {
            if (!pclosing)
                await searchStudentService.StudentStartedVideo(vurl);
            else
                await searchStudentService.StudentStoppedVideo(vurl);

            ReturnUrl = vurl;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            return RedirectToPage("./Index");
        }
    }
}
