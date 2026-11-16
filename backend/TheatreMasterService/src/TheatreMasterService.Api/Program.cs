using TheatreMasterService.Api.Extensions;
using TheatreMasterService.Application.Extensions;
using TheatreMasterService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Modular Clean Architecture Registrations
builder.Services.AddApiControllers();
builder.Services.AddApiJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();