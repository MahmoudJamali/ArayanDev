using DataAccess.Abstract.Context;
using Microsoft.EntityFrameworkCore;
using Business.Extentions;
using FluentValidation.AspNetCore;

using DataAccess.Concrete.Contexts;
using Business.Services;
using MediatR;
using Business.Handlers.Authentication.Commands;
using DataAccess.Abstract;
using DataAccess.Concrete.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDataAccessServices(builder.Configuration);
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddHttpClient<IOtpService, OtpService>();
// MediatR registration
builder.Services.AddCustomMediatR();
builder.Services.AddControllersWithViews();
builder.Services.AddFluentValidationAutoValidation();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "MyCookieAuth";
    options.DefaultSignInScheme = "MyCookieAuth";
    options.DefaultChallengeScheme = "MyCookieAuth";
})
.AddCookie("MyCookieAuth", options =>
{
    options.LoginPath = "/Auth/Login";
    // این مسیر جدید برای کاربرانی که لاگین هستند ولی پروفایل ناقص دارند
    options.AccessDeniedPath = "/Auth/ProfileIncomplete";
    options.ReturnUrlParameter = "returnUrl";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});


builder.Services.AddAuthorization(options =>
{
    // پالیسی: فقط زمانی اجرا شود که Claim پروفایل کامل باشد
    options.AddPolicy("CompleteProfile", policy =>
        policy.RequireClaim("ProfileCompleted", "true"));
});


builder.Services.AddAuthorization();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOtpRepository, OtpRepository>();

builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await RoleSeeder.SeedAsync(db);
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
