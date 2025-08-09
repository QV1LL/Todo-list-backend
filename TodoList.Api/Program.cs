using Microsoft.EntityFrameworkCore;
using TodoList.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);
SQLitePCL.Batteries.Init();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<TodoListContext>(options =>
{
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

//var allowedOrigins = builder.Configuration.GetSection("AllowedCorsOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

using(var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<TodoListContext>().Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();