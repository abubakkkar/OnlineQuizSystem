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

        public SignupModel()
        {
            _userService = new UserService();
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Required]
            public string Name { get; set; } = null!;

            [Required]
            [Display(Name = "Roll Number")]
            public string RollNo { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; } = null!;

            [Required]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            public string Password { get; set; } = null!;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = null!;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(Input.RollNo))
                {
                    ModelState.AddModelError("Input.RollNo", "Roll number is required.");
                    return Page();
                }

                try
                {
                    // Register user as a student with a user-entered roll number
                    _userService.Register(Input.Name, Input.Email, Input.Password, Input.RollNo, null);
                    return RedirectToPage("./Login");
                }
                catch (System.Exception)
                {
                    ModelState.AddModelError(string.Empty, "Registration failed. Ensure email is unique or database is connected.");
                }
            }

            return Page();
        }
    }
}
