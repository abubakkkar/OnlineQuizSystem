using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.BLL.Services;
using System.ComponentModel.DataAnnotations;

namespace OnlineQuizSystem.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly UserService _userService;
        private readonly TeacherService _teacherService;

        public LoginModel()
        {
            _userService = new UserService();
            _teacherService = new TeacherService();
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                // Check Users table first (Students)
                var userTable = _userService.Login(Input.Email, Input.Password);
                if (userTable.Rows.Count > 0)
                {
                    var user = userTable.Rows[0];
                    if (user.Table.Columns.Contains("IsActive") && !Convert.ToBoolean(user["IsActive"]))
                    {
                        ErrorMessage = "Your account has been disabled. Please contact the administrator.";
                        return Page();
                    }

                    HttpContext.Session.SetString("UserID", user["UserID"].ToString());
                    HttpContext.Session.SetString("Name", user["Name"].ToString());
                    HttpContext.Session.SetString("Role", user["Role"].ToString());
                    HttpContext.Session.SetString("Email", user["Email"].ToString());
                    
                    return RedirectToPage("/Dashboard/Index");
                }

                // Check Teachers table
                var teacherTable = _teacherService.Login(Input.Email, Input.Password);
                if (teacherTable.Rows.Count > 0)
                {
                    var teacher = teacherTable.Rows[0];
                    if (teacher.Table.Columns.Contains("IsActive") && !Convert.ToBoolean(teacher["IsActive"]))
                    {
                        ErrorMessage = "Your account has been disabled. Please contact the administrator.";
                        return Page();
                    }
                    HttpContext.Session.SetString("TeacherID", teacher["TeacherID"].ToString());
                    HttpContext.Session.SetString("Name", teacher["Name"].ToString());
                    HttpContext.Session.SetString("Role", teacher["Role"].ToString());
                    HttpContext.Session.SetString("Email", teacher["Email"].ToString());
                    
                    return RedirectToPage("/Teacher/Index");
                }
                
                ErrorMessage = "Invalid email or password.";
            }

            return Page();
        }
    }
}
