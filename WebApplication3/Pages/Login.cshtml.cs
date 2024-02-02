using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Model;
using WebApplication3.ViewModels;

namespace WebApplication3.Pages
{
    public class LoginModel : PageModel
    {

		[BindProperty]
		public Login LModel { get; set; }
		private readonly AuthDbContext dbContext;
		public DbSet<UserActivity> UserActivities { get; set; }
		private readonly SignInManager<ApplicationUser> signInManager;
		private UserManager<ApplicationUser> userManager { get; }
		private readonly IHttpContextAccessor httpContextAccessor;
		public LoginModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, AuthDbContext dbContext, UserManager<ApplicationUser> userManager)
		{
			this.signInManager = signInManager;
			this.httpContextAccessor = httpContextAccessor;
			this.dbContext = dbContext;
			this.userManager = userManager;
		}
		public void OnGet()
        {
        }

        //public async Task<IActionResult> OnPostAsync()
        //{
        //	if (ModelState.IsValid)
        //	{
        //		var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
        //			LModel.RememberMe, false);
        //		if (identityResult.Succeeded)
        //		{
        //			// Retrieve the user by email
        //			var user = await userManager.FindByEmailAsync(LModel.Email);

        //			if (user != null)
        //			{
        //				// Log user login activity
        //				LogUserActivity(user.Id, "Login", user.Email);

        //				// Store session upon successful login
        //				httpContextAccessor.HttpContext.Session.SetString("UserName", LModel.Email);

        //				return RedirectToPage("Index");
        //			}
        //			else
        //			{
        //				ModelState.AddModelError("", "User not found");
        //			}
        //		}
        //		ModelState.AddModelError("", "Username or Password incorrect");
        //	}
        //	return Page();
        //}

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
                    LModel.RememberMe, false);

                if (identityResult.Succeeded)
                {
                    // Retrieve the user by email
                    var user = await userManager.FindByEmailAsync(LModel.Email);

                    if (user != null)
                    {
                        // Store session upon successful login
                        httpContextAccessor.HttpContext.Session.SetString("UserName", LModel.Email);

                        // Check if the user's password is more than 2 minutes old
                        if (user.LastPasswordChange.HasValue && (DateTime.UtcNow - user.LastPasswordChange.Value).TotalMinutes > 2)
                        {
                            // Redirect to the ChangePassword page
                            return RedirectToPage("ChangePassword");
                        }

                        // Log user login activity
                        LogUserActivity(user.Id, "Login", user.Email);

                        return RedirectToPage("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "User not found");
                    }
                }
                ModelState.AddModelError("", "Username or Password incorrect");
            }
            return Page();
        }



        private void LogUserActivity(string userId, string activityType, string email)
		{

			// Create a new UserActivity entry
			var userActivity = new UserActivity
			{
				UserId = userId,
				Email = email,
				ActivityType = activityType,
				Timestamp = DateTime.Now
			};

			// Add the entry to the database
			dbContext.UserActivities.Add(userActivity);
			dbContext.SaveChanges();
		}


	}
}
