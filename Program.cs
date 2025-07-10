using HotelManagementSite.Data;
using HotelManagementSite.Interfaces;
using HotelManagementSite.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<HotelAuthDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("HotelAuthConnection")));
builder.Services.AddDbContext<HotelDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("HotelConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<HotelAuthDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthAccountRepository, AuthAccountRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddAuthentication()
        .AddGoogle(Options =>
        {
            builder.Configuration.GetSection("AuthenticationOptions:Google").Bind(Options); // client id and client secret 

            Options.Scope.Add("profile");
            Options.ClaimActions.MapJsonKey("picture", "picture", "url"); // for getting the image 

            Options.CallbackPath = "/signin-google";
            // these are needed to escape error if user cancels the external login without these app throws an exception
            Options.Events = new OAuthEvents
            {
                OnRemoteFailure = context =>
                {
                    context.Response.Redirect("/Account/LogIn?error=external_login_cancelled"); // for escaping error if user cancels the external login
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        })
        .AddFacebook(Options =>
        {
            builder.Configuration.GetSection("AuthenticationOptions:Facebook").Bind(Options);

            // additional information that we want to get from the user
            // Facebook requires the scope to be set for the additional information
            // if you want to get the profile picture and birthday, you need to add the fields
            Options.Scope.Add("public_profile");
            Options.Scope.Add("email");

            Options.Fields.Add("picture"); // currently this is not working, but it is needed to get the profile picture
            Options.Fields.Add("birthday");//currently this is not working, but it is needed to get the birthday



            Options.CallbackPath = "/signin-facebook";
            Options.Events = new OAuthEvents
            {
                OnRemoteFailure = context =>
                {
                    context.Response.Redirect("/Account/LogIn?error=external_login_cancelled"); // see google part of this page for explanation
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        })
        .AddLinkedIn(Options =>
        {
            builder.Configuration.GetSection("AuthenticationOptions:LinkedIn").Bind(Options);
        //     Options.Scope.Add("r_liteprofile");
        //     Options.Scope.Add("r_emailaddress");
        //    Options.ClaimActions.MapJsonKey("profilePicture", "profilePicture");//currently this is not working, but it is needed to get the profile picture
        //     Options.ClaimActions.MapJsonKey("emailAddress", "emailAddress", "email");

            // Set the UserInformationEndpoint to retrieve profile data
            // This URL often returns the actual JSON with profile information
            // Options.UserInformationEndpoint = "https://api.linkedin.com/v2/me?projection=(id,localizedFirstName,localizedLastName,profilePicture(displayImage~:playableStreams))";

            Options.CallbackPath = "/signin-linkedin";
            Options.Events = new OAuthEvents
            {
                OnRemoteFailure = context =>
                {
                    context.Response.Redirect("/Account/LogIn?error=external_login_cancelled");// see google part of this page for explanation
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };

        })
        .AddGitHub(Options =>
        {
            builder.Configuration.GetSection("AuthenticationOptions:GitHub").Bind(Options);
            Options.CallbackPath = "/signin-github";

            Options.Scope.Add("read:user");
            Options.Scope.Add("user:email");

            Options.ClaimActions.MapJsonKey("urn:github:login", "login");
            Options.ClaimActions.MapJsonKey("urn:github:avatarurl", "avatar_url");



            Options.Events = new OAuthEvents
            {
                OnRemoteFailure = context =>
                {
                    context.Response.Redirect("/Account/LogIn?error=external_login_cancelled");// see google part of this page for explanation
                    context.HandleResponse();
                    return Task.CompletedTask;
                }
            };
        });


//
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/LogIn"; 
    options.LogoutPath = "/Account/LogOut"; 
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // this is needed to set the cookie expiration time, if you want to use remember me functionality
    options.SlidingExpiration = true; // this is needed to reset the cookie expiration time
    options.Cookie.IsEssential = true; // this is needed to make the cookie essential, so that it is not blocked by the browser

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
app.UseStatusCodePagesWithRedirects("/Home/NotFound?code={0}");
app.UseStaticFiles(); // this line is needed to serve static files like css, js, images, etc.  need to be before UseRouting
app.MapStaticAssets();
app.UseRouting(); // this line is need to be after UseStaticFiles 
app.UseAuthentication();// this line is needed to be before UseAuthorization
app.UseAuthorization(); // this line need to be between useRouting and useEndpoints and after UseAuthentication 
app.UseEndpoints(endpoints => { _ = endpoints.MapDefaultControllerRoute(); }); // this line is needed to be after UseRouting

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
