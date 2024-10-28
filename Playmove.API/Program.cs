using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Playmove.API.Util;
using Playmove.DAO.Data;
using Playmove.DAO.Generic.Interface;
using Playmove.DAO.IService;
using Playmove.DAO.Models;
using Playmove.DAO.Service;
using Playmove.DAO.Service.Token;
using Playmove.Util;
using System.ComponentModel;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager config = builder.Configuration;
var env = builder.Environment;

#region Service


builder.Services.AddScoped<IServiceFornecedor, ServiceFornecedor>();
builder.Services.AddScoped<IServiceToken, ServiceToken>();


#endregion

#region Repository

builder.Services.AddScoped<IRepositoryFornecedor, RepositoryFornecedor>();
#endregion


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<FornecedoresDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<FornecedoresDbContext>();
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
      options.SerializerSettings.ReferenceLoopHandling =
        Newtonsoft.Json.ReferenceLoopHandling.Ignore);


builder.Services.AddSwaggerGen(
c =>
{
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

    c.SwaggerDoc("v.1.0", new OpenApiInfo
    {
        Title = "Playmove.API",
        Description = "API com autenticação JWT para controle de fornecedores",
        Contact = new OpenApiContact() { Name = "Paulo Henrique Malta Quintino de Araújo", Email = "phmqa@hotmail.com" },
    });
    c.IncludeXmlComments(Path.Combine(System.AppContext.BaseDirectory, "DemoSwaggerAnnotation.xml"));}
);

var services = builder.Services;


services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = false;
});

services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = long.MaxValue;
    options.MultipartBoundaryLengthLimit = int.MaxValue;
    options.MultipartHeadersCountLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

services.Configure<DataProtectionTokenProviderOptions>(options =>
    options.TokenLifespan = TimeSpan.FromMinutes(5)
);

services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
        builder.WithOrigins(config.GetProperty<string>("AppUrl", env.EnvironmentName))
        .SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
    );
});


services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.GetValue<string>("Secret"))),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
    };
    x.Events = new JwtBearerEvents

    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["access_token"];

            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            var response = context.HttpContext.Response;
            return Task.CompletedTask;
        }
    };
});

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.LoginPath = "/auth/login";
    options.LogoutPath = "/auth/logout";
    options.Events = new CookieAuthenticationEvents()
    {
        OnRedirectToLogin = redirectContext =>
        {
            return Task.CompletedTask;

        },
        OnRedirectToAccessDenied = redirectContext =>
        {
            return Task.CompletedTask;

        },
    };
});

services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddAuthenticationSchemes("Bearer", "Cookies")
        .Build();
});

//services.Configure<IdentityOptions>(options =>
//{
//    // Password settings  
//    options.Password.RequireDigit = true;
//    options.Password.RequiredLength = 8;
//    options.Password.RequireNonAlphanumeric = true;
//    options.Password.RequireUppercase = true;
//    options.Password.RequireLowercase = true;

//    // Lockout settings  
//    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
//    options.Lockout.MaxFailedAccessAttempts = 10;
//    options.Lockout.AllowedForNewUsers = true;

//    // User settings  
//    options.User.RequireUniqueEmail = true;
//});

services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = false;
    options.Cookie.SameSite = SameSiteMode.None;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c => 
c.SwaggerEndpoint("/swagger/v.1.0/swagger.json", "Playmove.API"));

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}/{nome?}");


app.MapRazorPages();

app.Run();
