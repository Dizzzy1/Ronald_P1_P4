using Ronald_P1_P4.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddScoped<AutorService>();
builder.Services.AddScoped<AutorService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var numbersService = scope.ServiceProvider
        .GetRequiredService<AutorService>();

    var autorService = scope.ServiceProvider
        .GetRequiredService<AutorService>();

    await numbersService.InitializeAsync();
    await autorService.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();