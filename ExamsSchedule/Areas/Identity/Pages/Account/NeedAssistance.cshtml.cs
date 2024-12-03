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
    public class NeedAssistanceModel : PageModel
    {
        public string SessionId { get; set; }

        SearchStudentService searchStudentService;

        public string Text { get; set; }

        public NeedAssistanceModel(SearchStudentService svc)
        {
            searchStudentService = svc;
        }

        [HttpGet]
        public async Task OnGetAsync(string sessionId = null)
        {
            SessionId = sessionId;

        }

        [HttpPost]
        public async Task OnPostSubmitAsync(string text, string sessionId)
        {
           await searchStudentService.UpdateNeedAssitanceStatus(true, long.Parse(sessionId));

            SessionId = sessionId;
            Text = text;
        }
    }
}
