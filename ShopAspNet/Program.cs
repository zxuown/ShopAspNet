using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopAspNet.Middlewares;
using ShopAspNet.Models;
using ShopAspNet.Models.LiqPaySDK;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ShopContext>(options =>
{
    options.UseSqlite("Data Source=ShopContext.db");
    SQLitePCL.Batteries.Init();
});
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<LiqPay>();
builder.Services.AddMvc();
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
	options.SignIn.RequireConfirmedPhoneNumber = false;
	options.SignIn.RequireConfirmedAccount = false;
	options.SignIn.RequireConfirmedEmail = false;

	options.Password.RequiredLength = 3;
	options.Password.RequiredUniqueChars = 0;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireDigit = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireUppercase = false;
})
	.AddRoles<IdentityRole<int>>()
	.AddEntityFrameworkStores<ShopContext>();
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseBuyerUid();
app.UseStaticFiles();
app.MapControllers();
app.MapControllerRoute(name: "default", pattern: "{controller=Category}/{action=Index}");
app.MapControllerRoute(name: "default", pattern: "{controller=Product}/{action=Index}");
//app.MapGet("/", () => "Hello World!");

app.Run();
