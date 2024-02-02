using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;

namespace WebApplication3.Pages
{
    [Authorize]
	[ValidateAntiForgeryToken]
	public class IndexModel : PageModel
    {
		public string DecryptedNRIC { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string DateOfBirth { get; set; }
        public string Resume { get; set; }
        public string WhoAmI { get; set; }
        private readonly ILogger<IndexModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public IndexModel(ILogger<IndexModel> logger, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
			_userManager = userManager;
			_httpContextAccessor = httpContextAccessor;

		}

        public void OnGet()
        {
            // Ensure that User is authenticated before proceeding
            if (User?.Identity?.IsAuthenticated != true)
            {
                return;
            }

            // Retrieve decrypted NRIC from user in the database
            var user = _userManager.GetUserAsync(User).Result;

            if (user != null)
            {
				_httpContextAccessor.HttpContext.Session.SetString("UserEmail", user.Email);
				_httpContextAccessor.HttpContext.Session.SetString("UserId", user.Id);


				var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");

                // Ensure NRIC is not null before attempting to unprotect
                if (!string.IsNullOrEmpty(user.NRIC))
                {
                    DecryptedNRIC = protector.Unprotect(user.NRIC);
                    //DecryptedNRIC = user.NRIC;
                }

                FirstName = user.FirstName;
                LastName = user.LastName;
                Gender = user.Gender;
                Email = user.Email;
                DateOfBirth = user.DateOfBirth;
                Resume = user.Resume;
                WhoAmI = user.WhoAmI;
            }
        }
    }
}