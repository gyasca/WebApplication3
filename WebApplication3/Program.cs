using AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Identity;
using WebApplication3.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
	options =>
	{
		// Password Settings
		options.Password.RequireDigit = true;
		options.Password.RequireLowercase = true;
		options.Password.RequireUppercase = true;
		options.Password.RequireNonAlphanumeric = true;
		options.Password.RequiredLength = 12;
		options.Password.RequiredUniqueChars = 1;

		// Account Lockout
		options.Lockout.AllowedForNewUsers = true;
		options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
		options.Lockout.MaxFailedAccessAttempts = 3;
	}).AddEntityFrameworkStores<AuthDbContext>();
builder.Services.AddDataProtection();
builder.Services.ConfigureApplicationCookie(options =>
{
	options.ExpireTimeSpan = TimeSpan.FromSeconds(30);
	options.SlidingExpiration = true;
	options.LoginPath = "/login";
});

//builder.Services.ConfigureApplicationCookie(options =>
//{
//	options.LoginPath = "/Login";

//	// Set the Secure attribute to ensure the cookie is sent only over HTTPS
//	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

//	// Set the SameSite attribute because got cookie error
//	options.Cookie.SameSite = SameSiteMode.None;

//});


// Session management
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache(); //save session in memory
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
