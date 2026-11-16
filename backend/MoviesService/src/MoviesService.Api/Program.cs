using FluentValidation.AspNetCore;
using MovieService.Infrastructure.Extensions;
using MoviesService.Api.Extensions;
using MoviesService.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// API Controllers & Routing Setup
builder.Services.AddApiControllers();
builder.Services.AddFluentValidationAutoValidation();

// Modular Clean Architecture Service Registrations
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // For serving static files like uploaded images
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();