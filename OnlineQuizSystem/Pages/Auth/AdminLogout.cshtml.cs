using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlineQuizSystem.Pages.Auth
{
    public class AdminLogoutModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Clear admin session
            HttpContext.Session.Remove("AdminID");
            HttpContext.Session.Remove("AdminName");
            HttpContext.Session.Remove("AdminRole");
            HttpContext.Session.Remove("AdminEmail");
            
            return RedirectToPage("/Index");
        }
    }
}
