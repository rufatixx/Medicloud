using Medicloud.BLL.Service;
using Medicloud.BLL.Service.Communication;

using Medicloud.BLL.Service.Organization;
using Medicloud.BLL.Services;
using Medicloud.BLL.Services.Abstract;
using Medicloud.BLL.Services.Concrete;
using Medicloud.BLL.Services.WorkHour;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Infrastructure.Concrete;
using Medicloud.DAL.Repository.Abstract;
using Medicloud.DAL.Repository.Concrete;
using Medicloud.DAL.Repository.Kassa;
using Medicloud.DAL.Repository.Organization;
using Medicloud.DAL.Repository.Patient;
using Medicloud.DAL.Repository.PatientCard;
using Medicloud.DAL.Repository.Plan;
using Medicloud.DAL.Repository.RequestType;
using Medicloud.DAL.Repository.Role;
using Medicloud.DAL.Repository.UserOrganization;
using Medicloud.DAL.Repository.UserPlan;
using Medicloud.DAL.Repository.Users;
using Medicloud.DAL.Repository.WorkHour;
using Medicloud.Data;
using Medicloud.Models.Repository;
using Medicloud.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();


// Register dependencies
builder.Services.AddScoped<SpecialityRepo>(provider => new SpecialityRepo(builder.Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value));
builder.Services.AddScoped<SpecialityService>();
builder.Services.AddScoped(provider => new HttpClient());
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//.AddCookie(options =>
//{
//    options.Cookie.HttpOnly = true;
//    options.ExpireTimeSpan = TimeSpan.FromDays(365 * 20); // Expire cookies in 20 years
//    options.SlidingExpiration = true; // Refresh expiration time if the user is active
//    options.LoginPath = "/Login/Index"; // Redirect to login page if not authenticated
//                                        // Add event handler to validate the principal
//                                        // Add event handler to validate the principal
//    options.Events = new CookieAuthenticationEvents
//    {
//        OnValidatePrincipal = async context =>
//        {


//            // Define the prefix for the authentication cookies
//            var appCookieExists = context.Request.Cookies.Keys.Any(key => key.StartsWith("Medicloud_"));

//            if (!appCookieExists)
//            {
//                // If no non-authentication cookies are found, reject the principal and sign out
//                context.RejectPrincipal();
//                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//                // Optionally, you can redirect the user to a login page or show a message
//                context.Response.Redirect("/Login/Index");
//            }
//        }
//    };

//});

// 1) Services and authentication setup
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Welcome/Index";
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.Cookie.HttpOnly = true;
        options.SlidingExpiration = true;

        // Remove or omit OnValidatePrincipal since we're doing checks in a pipeline step
    });
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.MaxAge = TimeSpan.FromDays(30); // Make session cookie persistent for 30 days
});


builder.Services.AddSwaggerGen();

var con= builder.Configuration.GetConnectionString("DefaultConnectionString");
builder.Services.AddScoped<IUnitOfWork>(provider => new UnitOfWork(con));
builder.Services.AddScoped<IServicesRepository, ServicesRepository>();
builder.Services.AddScoped<IServicesService, ServicesService>();
builder.Services.AddScoped<IRequestTypeService, RequestTypeService>();
builder.Services.AddScoped<IRequestTypeRepository, RequestTypeRepository>();
builder.Services.AddScoped<IPatientCardRepository, PatientCardRepository>();
builder.Services.AddScoped<IPatientCardService, PatientCardService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IServicePriceGroupRepository, ServicePriceGroupRepository>();
builder.Services.AddScoped<IOrganizationRepo, OrganizationRepo>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IUserPlanRepo, UserPlanRepo>();
builder.Services.AddScoped<IUserOrganizationRelRepository, UserOrganizationRelRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IKassaRepo, KassaRepo>();
builder.Services.AddScoped<ICommunicationService, CommunicationService>();
builder.Services.AddScoped<IPatientCardServiceRelRepository, PatientCardServiceRelRepository>();
builder.Services.AddScoped<IWorkHourRepository, WorkHourRepository>();
builder.Services.AddScoped<IWorkHourService, WorkHourService>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();


var app = builder.Build();

app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();



app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    // 1) If request is for NoRoleView, skip checks so user can see the page
    var path = context.Request.Path.Value?.ToLower() ?? "";
  
    if (path.StartsWith("/home/noroleview") ||
        path.StartsWith("/login") ||
        path.StartsWith("/profile")||
        path.StartsWith("/home") ||
                path.StartsWith("/pricing") ||
                path.StartsWith("/payment") ||
                path.StartsWith("/login")
        )
    {
        await next();
        return;
    }


    if (context.User.Identity?.IsAuthenticated == true)
    {
        // Check if session keys exist
        var hasMedicloudSession = context.Session.Keys.Any(k => k.StartsWith("Medicloud_"));
        if (!hasMedicloudSession)
        {
            // Sign out & redirect
            context.Response.Cookies.Delete(CookieAuthenticationDefaults.AuthenticationScheme);
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.Response.Redirect("/Login/Index");
            return;
        }

        // Check user roles
        var userIDStr = context.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0";
        var userID = Convert.ToInt32(userIDStr);

        var orgIDStr = context.Session.GetString("Medicloud_organizationID");
        var organizationID = string.IsNullOrEmpty(orgIDStr) ? 0 : Convert.ToInt32(orgIDStr);

        var roleRepo = context.RequestServices.GetRequiredService<IRoleRepository>();
        var userRoles = await roleRepo.GetUserRoles(organizationID, userID);

        if (!userRoles.Any())
        {
            // No roles => redirect to NoRoleView
            context.Response.Redirect("/Home/NoRoleView");
            return;
        }
    }

    await next();
});


app.Use(async (context, next) =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        var planExpiryDateString = context.Session.GetString("Medicloud_UserPlanExpireDate");
        if (string.IsNullOrEmpty(planExpiryDateString)
            || !DateTime.TryParse(planExpiryDateString, out var planExpiryDate)
            || DateTime.Now > planExpiryDate)
        {
            var path = context.Request.Path.Value.ToLower();

            if (path != "/" &&
                !path.StartsWith("/home") &&
                !path.StartsWith("/profile") &&
                !path.StartsWith("/pricing") &&
                !path.StartsWith("/payment") &&
                !path.StartsWith("/login"))
            {
                context.Response.Redirect("/Pricing");
                return;
            }
        }
    }
    await next();
});


//app.MapAreaControllerRoute(
//    name: "Admin",
//    areaName: "Admin",
//    pattern: "Admin/{controller=Home}/{action=Index}"
//);
app.MapControllerRoute(
        name: "areaRoute",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Map all API endpoints
    endpoints.MapControllers();
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Welcome}/{action=Index}/{id?}");
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flexit HIS API V1"); });

app.Run();

