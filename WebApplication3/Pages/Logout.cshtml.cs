using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Model;

namespace WebApplication3.Pages
{
    public class LogoutModel : PageModel
    {
		private readonly SignInManager<ApplicationUser> signInManager;
		private UserManager<ApplicationUser> userManager { get; }
		private readonly AuthDbContext dbContext;

		private readonly IHttpContextAccessor httpContextAccessor;
		public LogoutModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, AuthDbContext dbContext)
		{
			this.signInManager = signInManager;
			this.httpContextAccessor = httpContextAccessor;
			this.userManager = userManager;
			this.dbContext = dbContext;
		}

		public void OnGet()
        {
        }

		public async Task<IActionResult> OnPostLogoutAsync()
		{
			// Retrieve decrypted NRIC from user in the database
			var user = userManager.GetUserAsync(User).Result;

			// Log user registration activity
			LogUserActivity(user.Id, "Logout", user.Email);

			await signInManager.SignOutAsync();

			// Remove session upon logout
			httpContextAccessor.HttpContext.Session.Remove("UserName");

			return RedirectToPage("Login");
		}
		public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("Index");
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
