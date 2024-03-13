using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Villa.Application.Common.Interfaces;
using Villa.Domain.Entities;
using Villa.Infrastructure.Data;
using Villa.Infrastructure.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();
builder.Services.Configure<IdentityOptions>(option =>
{
    option.Password.RequiredLength = 8;
    option.Password.RequireNonAlphanumeric = false;
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAppUserRepo, AppUserRepo>();
var app = builder.Build();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
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