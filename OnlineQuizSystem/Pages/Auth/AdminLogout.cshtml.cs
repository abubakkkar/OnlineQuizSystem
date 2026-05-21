using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlineQuizSystem.Pages.Auth
{
    public class AdminLogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }

        public IActionResult OnPost()
        {
            // Clear all session data
            HttpContext.Session.Clear();
            
            return RedirectToPage("/Index");
        }
    }
}
