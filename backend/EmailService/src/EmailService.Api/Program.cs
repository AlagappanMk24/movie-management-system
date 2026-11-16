using EmailService.Api.Extensions;
using EmailService.Application.Extensions;
using EmailService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Modular Clean Architecture Registrations (Clean & Duplicate-Free)
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();