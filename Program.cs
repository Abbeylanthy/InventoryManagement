using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Data;
using InventoryManagement.Repositories;
using InventoryManagement.Repositories.Interfaces;
using InventoryManagement.Services;
using InventoryManagement.Services.Interfaces;
using InventoryManagement.Common.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;
using InventoryManagement.Entities;
using Microsoft.AspNetCore.Builder;


var builder = WebApplication.CreateBuilder(args);

// ===============================================
// Register Services
// ===============================================

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddHangfireServer();
// Controllers
builder.Services.AddControllers();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
       options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,

    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

    RoleClaimType = System.Security.Claims.ClaimTypes.Role
};
    });

// Authorization
builder.Services.AddAuthorization();

// Swagger + JWT Support
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });
    option.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer",document)] = []
    });
});

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddHttpContextAccessor();

// Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IInventoryMonitoringService, InventoryMonitoringService>();


// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider
        .GetRequiredService<IRecurringJobManager>(); // Get Hangfire's recurring job manager from DI

    recurringJobManager.AddOrUpdate<IInventoryMonitoringService>(
        "low-stock-check",
        service => service.CheckLowStockProductsAsync(),
        Cron.Minutely
    ); 
}
// Swagger (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();   
app.UseAuthorization();
app.UseHangfireDashboard();

app.MapControllers();

app.Run();