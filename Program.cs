using HotelManagementSite.Data;
using HotelManagementSite.interfaces;
using HotelManagementSite.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<HotelDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<HotelAuthDbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("HotelAuthConnection")));
builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<HotelAuthDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();



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

app.UseStaticFiles();
app.MapStaticAssets();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope =app.Services.CreateScope()){
    var services = scope.ServiceProvider;
    await AuthDbRoleAdminSeeder.SeedAdminAndRolesAsync(services);
}

app.Run();
