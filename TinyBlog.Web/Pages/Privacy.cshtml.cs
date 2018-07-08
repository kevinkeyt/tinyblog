﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TinyBlog.Web.Pages
{
    public class PrivacyModel : PageModel
    {
        public IActionResult OnGetAsync()
        {
            return Page();
        }
    }
}