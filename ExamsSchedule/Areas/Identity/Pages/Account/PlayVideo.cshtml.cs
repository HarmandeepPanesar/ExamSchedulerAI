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

namespace ExamsSchedule.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class PlayVideoModel : PageModel
    {
        public string ReturnUrl { get; set; }


        public async Task OnGetAsync(string vurl = null)
        {
            ReturnUrl = vurl;
        }
    }
}
