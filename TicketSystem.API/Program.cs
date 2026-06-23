using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Infrastructure.Database;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.IdentityModel.Tokens;
using TicketSystem.Application;
using TicketSystem.Infrastructure;
using TicketSystem.Infrastructure.Database;
using TicketSystem.Infrastructure.Notifications.Hubs;
using System.Text;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Backplane.StackExchangeRedis;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// 1. Aspire i osnovni servisi
builder.AddServiceDefaults();
builder.Services.AddApplication();
builder.AddDatabase();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Konfiguracija FusionCache-a i Redis-a
string? redisConnectionString = builder.Configuration["Redis:ConnectionString"];

var fusionCacheBuilder = builder.Services.AddFusionCache()
    .WithDefaultEntryOptions(option =>
    {
        option.Duration = TimeSpan.FromMinutes(5);
        option.DistributedCacheDuration = TimeSpan.FromMinutes(5);
    })
    .WithSerializer(new FusionCacheSystemTextJsonSerializer());

if (!string.IsNullOrWhiteSpace(redisConnectionString))
{
    fusionCacheBuilder
        .WithDistributedCache(new RedisCache(new RedisCacheOptions
        {
            Configuration = redisConnectionString
        }))
        .WithBackplane(new RedisBackplane(new RedisBackplaneOptions
        {
            Configuration = redisConnectionString
        }));
}

fusionCacheBuilder.AsHybridCache();

// 3. Kontroleri i OpenAPI / Swagger
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 4. FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(TicketSystem.Application.DependencyInjection).Assembly);
builder.Services.AddFluentValidationAutoValidation();

// 5. Autorizacija i CORS konfiguracija
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 6. Autentifikacija (Google & JWT Bearer)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    options.CallbackPath = builder.Configuration["Authentication:Google:CallbackPath"]!;
    options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
})
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };

    options.Events = new JwtBearerEvents
    {
        // SignalR WebSocket šalje token kao query string: ?access_token=...
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                context.Token = accessToken;
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\": \"Niste autorizovani (JWT nedostaje ili je istekao)\"}");
        }
    };
});

// ==========================================
// IZGRADNJA APLIKACIJE (HTTP PIPELINE)
// ==========================================
WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TicketSystem API v1");
        options.RoutePrefix = "swagger";
    });
}

// ISPRAVKA REDOSLEDA: Prvo podižemo ruting da bi .NET mapirao rute kontrolera
app.UseRouting();

// ISPRAVKA POLISE: Eksplicitno aktiviramo "AllowAll" polisu odmah nakon rutinga
// Ovo presreće pretraživačev OPTIONS zahtev i odobrava ga pre nego što stigne do kontrolera
app.UseCors("AllowAll");

// Autentifikacija i Autorizacija idu tek nakon što je CORS propustio zahtev
app.UseAuthentication();
app.UseAuthorization();

// Mapiranje krajnjih tačaka (endpoints)
app.MapControllers();
app.MapHub<TicketHub>("/hubs/tickets");

app.Run();