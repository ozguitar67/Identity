using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web_App.Data;
using Web_App.Data.Account;
using Web_App.Services;
using Web_App.Settings;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
    
    });

builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
    });
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = builder.Configuration["FacebookAppId"];
    options.AppSecret = builder.Configuration["FacebookAppSecret"];
});
builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
    .AddDefaultTokenProviders();
// Add services to the container
builder.Services.Configure<SmtpSetting>(builder.Configuration.GetSection("SMTP"));
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddRazorPages();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllers();
});

app.Run();
