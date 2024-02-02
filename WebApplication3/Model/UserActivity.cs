using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication3.Model
{
	public class UserActivity
	{
		[Key]
		public int Id { get; set; }

		public string UserId { get; set; } // Foreign key to ApplicationUser

		[ForeignKey("UserId")]
		public ApplicationUser User { get; set; } // Navigation property to ApplicationUser (UserName == Email)

		public string Email { get; set; }

		public string ActivityType { get; set; }

		public DateTime Timestamp { get; set; }
	}
}
