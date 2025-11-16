using FinancyAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Register our services (Dependency Injection)
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<BudgetService>();

var app = builder.Build();

app.UseCors("AllowAll");
app.MapControllers();

app.Run();