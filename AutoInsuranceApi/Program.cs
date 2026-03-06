using System.IO;
using System.Text;
using AutoInsuranceApi.Data;
using AutoInsuranceApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

// Serve client app from ../Client when running - set at creation to avoid NotSupportedException
var contentRoot = Directory.GetCurrentDirectory();
var clientPath = Path.GetFullPath(Path.Combine(contentRoot, "..", "Client"));
var options = new WebApplicationOptions
{
    Args = args,
    ContentRootPath = contentRoot,
    WebRootPath = Directory.Exists(clientPath) ? clientPath : contentRoot
};
var builder = WebApplication.CreateBuilder(options);

// SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=autoinsurance.db"));

// JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "AutoInsuranceSecretKeyThatIsAtLeast32CharactersLong!");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "AutoInsuranceApi",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "AutoInsuranceClient",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddControllers();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000", "http://localhost:8080", "http://127.0.0.1:8080", "http://localhost:5000", "https://localhost:5001", "http://localhost:5500", "http://127.0.0.1:5500")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();
// Handle CORS preflight (OPTIONS) for /api so response is 2xx with CORS headers (avoid redirect/fallback)
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS" && context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.StatusCode = 204;
        return;
    }
    await next();
});
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auto Insurance API v1"));
}

app.UseHttpsRedirection();

// Serve SPA static files and default/index
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
// SPA fallback: non-API routes serve index.html
app.MapFallbackToFile("index.html");

app.Run();
