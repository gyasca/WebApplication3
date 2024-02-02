using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using WebApplication3.Model;
using WebApplication3.ViewModels;

namespace WebApplication3.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AuthDbContext dbContext;
        private SignInManager<ApplicationUser> signInManager { get; }

        [BindProperty]
        public ChangePassword CModel { get; set; }

        public ChangePasswordModel(UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            AuthDbContext dbContext,
            SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
            this.signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);

                if (user == null)
                {
                    return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
                }

                // Check if the user changed the password within the last 2 minutes
                if (user.LastPasswordChange.HasValue && (DateTime.UtcNow - user.LastPasswordChange.Value).TotalMinutes < 2)
                {
                    ModelState.AddModelError(string.Empty, "You cannot change the password in the next 2 minutes.");
                    return Page();
                }

                var changePasswordResult = await userManager.ChangePasswordAsync(user, CModel.OldPassword, CModel.NewPassword);

                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                // update the LastPasswordChange property
                user.LastPasswordChange = DateTime.UtcNow;
                await userManager.UpdateAsync(user);

                // Log user activity upon successful password change
                LogUserActivity(user.Id, "ChangedPassword", user.Email);

                await signInManager.RefreshSignInAsync(user);
                return RedirectToPage("Index");
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
