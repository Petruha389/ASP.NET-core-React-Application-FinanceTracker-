using FinanceTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using FinanceTracker.Application.Extensions;
using Serilog;
using System.Text.Json.Serialization;

// ============ НАСТРОЙКА ЛОГГЕРА ============
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/finance-tracker-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Запуск приложения");

    var builder = WebApplication.CreateBuilder(args);

    // Добавляем хост Serilog
    builder.Host.UseSerilog();

    // 1. Добавляем MVC + API контроллеры с общими настройками JSON
    builder.Services.AddControllersWithViews()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    // 2. Аутентификация: Cookie для MVC, JWT для API
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // для MVC
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

    builder.Services.AddAuthorization();

    // 3. Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "FinanceTracker API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Введите JWT токен в формате: Bearer {token}",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // 4. База данных
    builder.Services.AddDbContext<FinanceTrackerDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

    // 5. Регистрация сервисов Application (включая PasswordHasher, если используете)
    builder.Services.AddApplicationServices();

    // 6. CORS (разрешаем React)
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowReact", policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    var app = builder.Build();

    // ============ ПАЙПЛАЙН ============

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinanceTracker API v1"));
    }
    else
    {
        // В production полагаемся на кастомный middleware (он перехватит все ошибки)
        // app.UseExceptionHandler("/Home/Error"); // Убираем, т.к. используем кастомный обработчик
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    // ====== КАСТОМНЫЙ ОБРАБОТЧИК ОШИБОК (должен быть до UseAuthentication) ======
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // CORS – после UseRouting, до UseAuthorization
    app.UseCors("AllowReact");

    app.UseAuthentication();
    app.UseAuthorization();

    // Маршруты – по умолчанию Dashboard
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Dashboard}/{action=Index}/{id?}");

    app.MapControllers(); // для API-контроллеров с атрибутами

    // ============ МИГРАЦИИ И SEED ============
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<FinanceTrackerDbContext>();
        await dbContext.Database.MigrateAsync();
        await FinanceTrackerDbContext.SeedAsync(dbContext);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Приложение завершилось с ошибкой");
}
finally
{
    Log.CloseAndFlush();
}