using System.Text;
using FluentValidation.AspNetCore;
using Library.Contracts.Books.Request;
using Library.Data.PostgreSql;
using Library.Data.PostgreSql.Repositories;
using Library.Documents.MongoDb.Repositories;
using Library.Domain.Abstractions.Services;
using Library.Domain.Abstractions.Storage;
using Library.Domain.Services;
using Library.Identity;
using Library.SharedKernel.Options;
using Library.Web.BackgroundServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddHealthChecks();

services.AddControllers()
    .AddFluentValidation(fv => 
    {
        fv.RegisterValidatorsFromAssemblyContaining<CreateBookRequest>();
        fv.AutomaticValidationEnabled = true;
    });
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddScoped<IBookService, BookService>();
services.AddScoped<IBookRepository, BookRepository>();

services.Configure<MySettings>(
    builder.Configuration.GetSection("MySettings"));

services.AddDbContext<BookContext>(options => 
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnectionString"));
});

services.AddStackExchangeRedisCache(options => { options.Configuration = "redis:6379"; });
services.AddOutputCache(options =>
{
    options.AddPolicy("BookPolicy", policy =>
    {
        policy.Expire(TimeSpan.FromSeconds(60));
    });
});

services.AddSingleton<IMongoClient>(new MongoClient("mongodb://mongo:27017"));

services.AddSingleton<IReviewService, ReviewService>();
services.AddSingleton<IReviewRepository, ReviewRepository>();

services.AddHostedService<AverageRatingCalculatorService>();

services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>()
    .AddDefaultTokenProviders();

services.AddDbContext<IdentityContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnectionString")));

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
        };
    });

services.AddScoped<JwtService>();

services.AddAuthorization(options =>
{
    options.AddPolicy("OlderThan18", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context =>
        {
            if (!context.User.HasClaim(c => c.Type == "DateOfBirth"))
                return false;

            var dobClaim = context.User.FindFirst("DateOfBirth")!.Value;
            if (!DateTime.TryParse(dobClaim, out var dob))
                return false;

            var age = DateTime.Today.Year - dob.Year;
            if (dob > DateTime.Today.AddYears(-age)) age--;

            return age >= 18;
        });
    });
});

var app = builder.Build();

var mySettings = app.Services.GetRequiredService<IOptions<MySettings>>().Value;

if (mySettings.EnableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var pds = context.RequestServices.GetService<IProblemDetailsService>();

        if (pds == null || !await pds.TryWriteAsync(new ProblemDetailsContext { HttpContext = context }))
        {
            await context.Response.WriteAsync("Fallback: An error occurred.");
        }
    });
});

app.Use(async (context, next) =>
{
    var startTime = DateTime.UtcNow;
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Запрос начал обрабатываться: {context.Request.Method} {context.Request.Path}");
    
    await next();
    
    var endTime = DateTime.UtcNow;
    var executionTime = endTime - startTime;
    
    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Запрос обработан: {context.Request.Method} {context.Request.Path} - " +
                      $"Время выполнения: {executionTime.TotalMilliseconds} мс - " +
                      $"Статус: {context.Response.StatusCode}");
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthz");

app.Run();