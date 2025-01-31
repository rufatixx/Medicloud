using System.Configuration;
using System.Text;
using Medicloud.BLL.Service;
using Medicloud.BLL.Services.Category;
using Medicloud.BLL.Services.Organization;
using Medicloud.BLL.Services.OTP;
using Medicloud.BLL.Services.PatientCard;
using Medicloud.BLL.Services.RequestType;
using Medicloud.BLL.Services.Services;
using Medicloud.BLL.Services.Staff;
using Medicloud.BLL.Services.User;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.Category;
using Medicloud.DAL.Repository.Organizationn;
using Medicloud.DAL.Repository.OrganizationTravelRel;
using Medicloud.DAL.Repository.OTP;
using Medicloud.DAL.Repository.PatientCard;
using Medicloud.DAL.Repository.RequestType;
using Medicloud.DAL.Repository.Role;
using Medicloud.DAL.Repository.Services;
using Medicloud.DAL.Repository.Staff;
using Medicloud.DAL.Repository.Userr;
using Medicloud.Data;
using Medicloud.WebUI.Middlewares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

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
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.MaxAge = TimeSpan.FromDays(30); // Make session cookie persistent for 30 days
});


var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtSettings:SecretKey").Value);



//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//	.AddJwtBearer(options =>
//	{
//		options.SaveToken = true;
//		options.TokenValidationParameters = new TokenValidationParameters
//		{
//			ValidateIssuerSigningKey = true,
//			IssuerSigningKey = new SymmetricSecurityKey(key),
//			ValidateIssuer = false,
//			ValidateAudience = false
//		};

//		options.Events = new JwtBearerEvents
//		{
//			OnAuthenticationFailed = context =>
//			{
//				Console.WriteLine("Authentication failed: " + context.Exception.Message);
//				var path = context.HttpContext.Request.Path;
//				var isApiRequest = path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase);

//				var isAjaxRequest = context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

//				if (isApiRequest)
//				{
//					Console.WriteLine("Authentication failed for an API request.");
//				}
//				else if (isAjaxRequest)
//				{
//					Console.WriteLine("Authentication failed for an AJAX request.");
//				}
//				else
//				{
//					Console.WriteLine("Authentication failed for a normal MVC request.");
//					context.HttpContext.Response.Redirect("/Login/Index");
//				}

//				//Console.WriteLine("Authentication failed: " + context.Exception.Message);

//				return Task.CompletedTask;
//			},
//			OnTokenValidated = context =>
//			{
//				//Console.WriteLine("Token is valid");

//				return Task.CompletedTask;
//			}
//		};
//	});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.SaveToken = true;
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(key),
			ValidateIssuer = false,
			ValidateAudience = false
		};

		options.Events = new JwtBearerEvents
		{
			OnAuthenticationFailed = context =>
			{
				Console.WriteLine("Authentication failed: " + context.Exception.Message);

				var path = context.HttpContext.Request.Path;
				var isApiRequest = path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase);
				var isAjaxRequest = context.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

				if (isApiRequest)
				{
					Console.WriteLine("Authentication failed for an API request.");
					context.HttpContext.Response.StatusCode = 401; // API istekleri için 401 dönülüyor
				}
				else if (isAjaxRequest)
				{
					Console.WriteLine("Authentication failed for an AJAX request.");
					context.HttpContext.Response.StatusCode = 401; // AJAX istekleri için de 401 dönülüyor
				}
				else
				{
					Console.WriteLine("Authentication failed for a normal MVC request.");
					context.HttpContext.Response.Redirect("/Login/Index"); // MVC istekleri için yönlendirme yapılır
				}

				return Task.CompletedTask;
			},
			OnTokenValidated = context =>
			{
				// Token geçerli olduğunda burada işlem yapılabilir
				return Task.CompletedTask;
			}
		};
	});

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/Login/Index";
//        options.ExpireTimeSpan = TimeSpan.FromDays(30); // Make sure this aligns with session cookie MaxAge
//        options.Cookie.HttpOnly = true;
//        options.SlidingExpiration = true; // Refreshes the expiration time if a request is made and more than half the ExpireTimeSpan has elapsed

//        // Добавляем обработку события перенаправления на страницу входа
//        options.Events = new CookieAuthenticationEvents
//        {
//            OnValidatePrincipal = async context =>
//                   {


//               // Получаем ключи всех сессий
//               //var sessionKeys = context.HttpContext.Session.Keys;

//               // // Проверяем, есть ли сессии, начинающиеся на Medicloud_
//               // var hasMedicloudSession = sessionKeys.Any(key => key.StartsWith("Medicloud_"));

//               // if (!hasMedicloudSession)
//               // {
//               //     // Если сессий Medicloud_ нет, очищаем cookie аутентификации и выполняем перенаправление на страницу выхода
//               //     context.HttpContext.Response.Cookies.Delete(CookieAuthenticationDefaults.AuthenticationScheme);
//               //     context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
//               //     context.Response.Redirect("/Login/Index");

//               // }



//            }
//        };
//    });

builder.Services.AddSwaggerGen();

var con= builder.Configuration.GetConnectionString("DefaultConnectionString");
builder.Services.AddScoped<IUnitOfWork>(provider => new UnitOfWork(con));
builder.Services.AddScoped<IServicesRepository, ServicesRepository>();
builder.Services.AddScoped<IServicesService, ServicesService>();
builder.Services.AddScoped<IRequestTypeRepository, RequestTypeRepository>();
builder.Services.AddScoped<IRequestTypeService, RequestTypeService>();
builder.Services.AddScoped<IPatientCardRepository, PatientCardRepository>();
builder.Services.AddScoped<IPatientCardService, PatientCardService>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<INUserService, NUserService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IOTPRepository, OTPRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IOrganizationTravelRelRepository, OrganizationTravelRelRepository>();

builder.Services.AddScoped<IOrganizationCategoryRelRepository, OrganizationCategoryRelRepository>();
builder.Services.AddScoped<IStaffWorkHoursRepository, StaffWorkHoursRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();
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


app.UseMiddleware<JwtTokenMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

//app.Use(async (context, next) =>
//{
//    if (context.User.Identity.IsAuthenticated)
//    {
//        var planExpiryDateString = context.Session.GetString("Medicloud_UserPlanExpireDate");

//        if (string.IsNullOrEmpty(planExpiryDateString) || !DateTime.TryParse(planExpiryDateString, out var planExpiryDate) || DateTime.Now > planExpiryDate)
//        {
//            var path = context.Request.Path.Value.ToLower();

//            if (path != "/" && !path.StartsWith("/home") && !path.StartsWith("/profile") && !path.StartsWith("/pricing") && !path.StartsWith("/payment") && !path.StartsWith("/login"))
//            {
//                context.Response.Redirect("/Pricing");
//                return;
//            }
//        }
//    }
//    await next();
//});

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
        pattern: "{controller=Welcome}/{action=Index}/{id?}");

    // Map all API endpoints
    endpoints.MapControllers();
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Welcome}/{action=Index}/{id?}");
app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flexit HIS API V1"); });

app.Run();

