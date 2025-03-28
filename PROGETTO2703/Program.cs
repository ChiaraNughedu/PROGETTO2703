using System.Security.Principal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PROGETTO2703.Data;
using PROGETTO2703.Models;
using PROGETTO2703.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<Identity>(builder.Configuration.GetSection(nameof(Identity)));
builder.Services.Configure<Jwt>(builder.Configuration.GetSection(nameof(Jwt)));
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


// registrare DbContext 
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options => {
    options.SignIn.RequireConfirmedAccount =
        builder.Configuration.GetValue<bool>("Identity:RequireConfirmedAccount");
    options.Password.RequiredLength =
        builder.Configuration.GetValue<int>("Identity:RequiredLength");
    options.Password.RequireDigit =
        builder.Configuration.GetValue<bool>("Identity:RequireDigit");
    options.Password.RequireLowercase =
        builder.Configuration.GetValue<bool>("Identity:RequireLowercase");
    options.Password.RequireNonAlphanumeric =
        builder.Configuration.GetValue<bool>("Identity:RequireNonAlphanumeric");
    options.Password.RequireUppercase =
        builder.Configuration.GetValue<bool>("Identity:RequireUppercase");
})
   .AddEntityFrameworkStores<ApplicationDbContext>()
   .AddDefaultTokenProviders();

// Configura JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection.GetValue<string>("Secret");
var issuer = jwtSection.GetValue<string>("Issuer");
var audience = jwtSection.GetValue<string>("Audience");

builder.Services.AddAuthentication(
    options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
     .AddJwtBearer(options => {
         options.TokenValidationParameters = new TokenValidationParameters
         {
             ValidateIssuer = true,
             ValidateAudience = true,
             ValidateLifetime = true,
             ValidateIssuerSigningKey = true,
             ValidIssuer = builder.Configuration["Jwt:Issuer"],
             ValidAudience = builder.Configuration["Jwt:Audience"],
             IssuerSigningKey = new SymmetricSecurityKey(
                 Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecurityKey"]))
         };
     });

builder.Services.AddScoped<UserManager<ApplicationUser>>();
builder.Services.AddScoped<SignInManager<ApplicationUser>>();
builder.Services.AddScoped<RoleManager<ApplicationRole>>();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Configure the HTTP request pipeline.


app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


// global cors policy
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.MapControllers();

app.Run();