// Program.cs
using FinancyAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Register your services
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<BudgetService>();

// CORRECT: register CategoryService (no ".cs")
builder.Services.AddScoped<CategoryService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();