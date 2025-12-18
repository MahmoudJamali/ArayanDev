using DataAccess.Abstract.Context;
using Microsoft.EntityFrameworkCore;
using Business.Extentions;
using Business.Services.Authentication;
using FluentValidation.AspNetCore;
using ArayanDev.Business.Handlers.Users.Commands;
using FluentValidation;
using DataAccess.Concrete.Contexts;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddDataAccessServices(builder.Configuration)
    .AddApplicationServices();

builder.Services.AddControllersWithViews();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserLoginCommandValidator>();







var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
