using BikeShare.Web.Models;
using BikeShare.Web.Services;
using BikeShare.Web.Services.Authentication;
using Dapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

SimpleCRUD.SetDialect(SimpleCRUD.Dialect.SQLite);
// SqlMapper.AddTypeHandler(new DateTimeToUnixHandler());
DefaultTypeMap.MatchNamesWithUnderscores = true;

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<StationService>();
builder.Services.AddScoped<BikeService>();
builder.Services.AddScoped<RentalService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.Configure<AuthorizationOptions>(options =>
{
    options.AddPolicy("Admin", p => p.Requirements.Add(new AdminRequirement()));
    options.AddPolicy("User", p => p.Requirements.Add(new UserRequirement()));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();