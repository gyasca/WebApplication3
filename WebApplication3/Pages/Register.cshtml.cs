using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Model;
using WebApplication3.ViewModels;

namespace WebApplication3.Pages
{
    public class RegisterModel : PageModel
    {
		private readonly AuthDbContext dbContext;

		private UserManager<ApplicationUser> userManager { get; }
        private SignInManager<ApplicationUser> signInManager { get; }

		private readonly IHttpContextAccessor httpContextAccessor;

		[BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
		IHttpContextAccessor httpContextAccessor,
		AuthDbContext dbContext)
        {
			this.userManager = userManager;
            this.signInManager = signInManager;
			this.httpContextAccessor = httpContextAccessor;
			this.dbContext = dbContext;
		}



        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");

                var user = new ApplicationUser()
                {
                    UserName = RModel.Email,
                    Email = RModel.Email,
					// new attributes
					FirstName = RModel.FirstName,
                    LastName = RModel.LastName,
                    Gender = RModel.Gender,
                    NRIC = protector.Protect(RModel.NRIC),
                    DateOfBirth = RModel.DateOfBirth,
                    Resume = RModel.Resume,
                    WhoAmI = RModel.WhoAmI,
                    LastPasswordChange = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    // Automatically confirm the email (For verification)
                    user.EmailConfirmed = true;

                    await signInManager.SignInAsync(user, false);

					// Store session upon successful registration
					httpContextAccessor.HttpContext.Session.SetString("UserName", RModel.Email);

					// Log user registration activity
					LogUserActivity(user.Id, "Registration", user.Email);

					return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }

		private void LogUserActivity(string userId, string activityType, string email)
		{
			var userActivity = new UserActivity
			{
				UserId = userId,
				Email = email,
				ActivityType = activityType,
				Timestamp = DateTime.UtcNow
			};

			dbContext.UserActivities.Add(userActivity);
			dbContext.SaveChanges();
		}

	}
}
