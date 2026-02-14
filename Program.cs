using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Poseidon.Configurations;
using Poseidon.Data;
using Poseidon.Endpoints;
using Poseidon.Repositories;
using Poseidon.Repositories.Interfaces;
using Poseidon.Services;
using Poseidon.Services.Interfaces;
using Resend;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Configuration.AddJsonFile("emailtemplates.json", optional: false, reloadOnChange: true);

builder.Services.Configure<AuthSetting>(
    builder.Configuration.GetSection("Authentication"));
builder.Services.Configure<InactivitySetting>(
    builder.Configuration.GetSection("InactivitySetting"));
builder.Services.Configure<List<EmailTemplateConfig>>(
    builder.Configuration.GetSection("EmailTemplates"));

//db connection
builder.Services.AddDbContext<PoseidonDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PoseidonDb"));
});

//other services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, ResendEmailService>();
builder.Services.AddScoped<IUserService, UserService>();

//Email service api
builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(o =>
{
    o.ApiToken = Environment.GetEnvironmentVariable("RESEND_APITOKEN")!;
});
builder.Services.AddTransient<IResend, ResendClient>();

var authsetting = builder.Configuration.GetSection("Authentication").Get<AuthSetting>();

//Authentication
builder.Services.AddAuthentication(authsetting.AuthScheme)
    .AddCookie(authsetting.AuthScheme, options =>
    {
        options.Cookie.Name = authsetting.CookieName;
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Redirect/AccessDenied";

        options.ExpireTimeSpan = TimeSpan.FromMinutes(authsetting.CookieExpireMinutes);
        options.SlidingExpiration = authsetting.UseSlidingExpiration;

        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                // Prevent redirect on AJAX or API requests
                if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
                    context.Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }

                // Default behavior (normal web request)
                context.Response.Redirect(context.RedirectUri);
                return Task.CompletedTask;
            }
        };

    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        context.Context.Response.Headers.Append("Cache-Control", "public,max-age=600");
        context.Context.Response.Headers.Append("Expires", DateTime.UtcNow.AddMinutes(10).ToString());
    }
});
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.MapStaticAssets();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}")
//    .WithStaticAssets();

app.UseAuthentication();

app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
