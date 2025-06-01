using HotelManagementSite.Data;
using HotelManagementSite.interfaces;
using HotelManagementSite.Repositories;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<HotelDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<HotelAuthDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("HotelAuthConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<HotelAuthDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddAuthentication()
        .AddGoogle(Options =>
        {
            builder.Configuration.GetSection("AuthenticationOptions:Google").Bind(Options);
            Options.CallbackPath = "/signin-google";
            Options.Events = new OAuthEvents
            {
                OnRemoteFailure = context =>
                {
                    context.Response.Redirect("/Account/LogIn?error=external_login_cancelled");
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        })
        .AddFacebook(Options =>
        {
            builder.Configuration.GetSection("AuthenticationOptions:Facebook").Bind(Options);
            Options.CallbackPath = "/signin-facebook";
            Options.Events = new OAuthEvents
            {
                OnRemoteFailure = context =>
                {
                    context.Response.Redirect("/Account/LogIn?error=external_login_cancelled");
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        })
        .AddLinkedIn(Options =>
        {
            builder.Configuration.GetSection("AuthenticationOptions:LinkedIn").Bind(Options);
            Options.CallbackPath = "/signin-linkedin";
            Options.Events = new OAuthEvents
            {
                OnRemoteFailure = context =>
                {
                    context.Response.Redirect("/Account/LogIn?error=external_login_cancelled");
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };

        })
        .AddGitHub(Options =>
        {
            builder.Configuration.GetSection("AuthenticationOptions:GitHub").Bind(Options);
            Options.CallbackPath = "/signin-github";
            Options.Scope.Add("user:email");
            Options.Events = new OAuthEvents
            {
                OnRemoteFailure = context =>
                {
                    context.Response.Redirect("/Account/LogIn?error=external_login_cancelled");
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
}
);



var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => { _ = endpoints.MapDefaultControllerRoute(); });

app.UseStaticFiles();
app.MapStaticAssets();




app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await AuthDbRoleAdminSeeder.SeedAdminAndRolesAsync(services);
}

app.Run();
