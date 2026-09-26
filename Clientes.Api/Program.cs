using Clientes.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<ClientesDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("Clientes")
        ?? throw new InvalidOperationException("Falta ConnectionStrings:Clientes.")));
var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API de clientes v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "API de clientes";
    });
}
app.MapControllers();
app.Run();
