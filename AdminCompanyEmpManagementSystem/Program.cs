using AdminCompanyEmpManagementSystem;
using AdminCompanyEmpManagementSystem.DTOMapping;
using AdminCompanyEmpManagementSystem.Identity;
using AdminCompanyEmpManagementSystem.Repository;
using AdminCompanyEmpManagementSystem.Repository.IRepository;
using AdminCompanyEmpManagementSystem.Services;
using AdminCompanyEmpManagementSystem.Services.IServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Data.Common;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// add the connection string

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"), b => b.MigrationsAssembly("AdminCompanyEmpManagementSystem"));

});
// set the configuration for identity implementation
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

// here add owr custom services 
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtManager, JwtManager>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


// adding jwt configuration

// jwt Configuration
//---here we will get the AppSettingJWT in appsetting.json 
var appsettingSection = builder.Configuration.GetSection("AppSettingJWT");
// here we will register the AppSettingJWT instance.
builder.Services.Configure<AppSettingJwt>(appsettingSection);
// here we will get the AppSettingJWT section that is in appsettingSection
var appsetting = appsettingSection.Get<AppSettingJwt>();
// here we will convert the secretKey to bytes and stroe in key.
var key = Encoding.ASCII.GetBytes(appsetting.SecretKey);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
    x.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
            }
            return Task.CompletedTask;
        }
    };
});



// set dto configuration
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

// adding cors policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
/*Here we will create the role by code */
IServiceScopeFactory serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (IServiceScope scope = serviceScopeFactory.CreateScope()) 
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    if(!await roleManager.RoleExistsAsync(SD.Role_Admin))
    {
        var role = new IdentityRole();
        role.Name = SD.Role_Admin;
        await roleManager.CreateAsync(role);
    }
    if(!await roleManager.RoleExistsAsync(SD.Role_Employee))
    {
        var role = new IdentityRole();
        role.Name = SD.Role_Employee;
        await roleManager.CreateAsync(role);
    }
    if(!await roleManager.RoleExistsAsync(SD.Role_Company))
    {
        var role = new IdentityRole();
        role.Name = SD.Role_Company;
        await roleManager.CreateAsync(role);
    }
}
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("MyPolicy");
app.MapControllers();

app.Run();
