using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineQuizSystem.BLL.Services;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace OnlineQuizSystem.Pages.Auth
{
    public class SignupModel : PageModel
    {
        private readonly UserService _userService;
        private readonly TeacherService _teacherService;

        public SignupModel()
        {
            _userService = new UserService();
            _teacherService = new TeacherService();
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public DataTable Teachers { get; set; }

        public class InputModel
        {
            [Required]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public int[] SelectedTeacherIds { get; set; } = new int[] { };
        }

        public void OnGet()
        {
            Teachers = _teacherService.GetAllTeachers();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Register user (without specifying a single teacher initially)
                    _userService.Register(Input.Name, Input.Email, Input.Password, null);
                    
                    // Get the newly registered user
                    var loginResult = _userService.Login(Input.Email, Input.Password);
                    if (loginResult.Rows.Count > 0)
                    {
                        int userId = Convert.ToInt32(loginResult.Rows[0]["UserID"]);
                        
                        // Enroll user in all selected teachers
                        if (Input.SelectedTeacherIds != null && Input.SelectedTeacherIds.Length > 0)
                        {
                            foreach (int teacherId in Input.SelectedTeacherIds)
                            {
                                _userService.EnrollInTeacher(userId, teacherId);
                            }
                        }
                    }
                    
                    return RedirectToPage("./Login");
                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Registration failed. Ensure email is unique or database is connected.");
                }
            }

            Teachers = _teacherService.GetAllTeachers();
            return Page();
        }
    }
}
