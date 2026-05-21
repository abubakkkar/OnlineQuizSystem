using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlineQuizSystem.Pages.Auth
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }

        public IActionResult OnPost()
        {
            HttpContext.Session.Clear();
            return RedirectToPage("/Index");
        }
    }
}
