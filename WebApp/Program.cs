using Microsoft.EntityFrameworkCore;
using Infrastructure.Contexts;
using Infrastructure.Entites;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();
builder.Services.AddDbContext<WebAppContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("WebAppDB")));
builder.Services.AddIdentity<UserEntity, IdentityRole>(x =>
{
	x.SignIn.RequireConfirmedAccount = false;
	x.User.RequireUniqueEmail = true;
	x.Password.RequiredLength = 8;
}).AddEntityFrameworkStores<WebAppContext>();

builder.Services.ConfigureApplicationCookie(x =>
{
	x.LoginPath = "/signin";
	x.Cookie.HttpOnly = true;
	x.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	x.ExpireTimeSpan = TimeSpan.FromHours(1);
	x.SlidingExpiration = true;
});



var app = builder.Build();
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Defalut}/{action=Home}/{id?}");

app.Run();
