using FluentValidation;
using System.Runtime.CompilerServices;
using victoria_tap.Controllers.Contracts;
using victoria_tap.Providers;
using victoria_tap.Providers.Contracts;
using victoria_tap.Repositories;
using victoria_tap.Repositories.Contracts;
using victoria_tap.Services;
using victoria_tap.Services.Contracts;
using victoria_tap.Validators;

[assembly: InternalsVisibleTo("victoria-tap-integration-tests")]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<VictoriaDbContext>(ServiceLifetime.Singleton);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI dotnet native: dependency injections allows us to correctly handle IoC
builder.Services.AddScoped<IVictoriaTapService, VictoriaTapService>();
builder.Services.AddSingleton<IVictoriaTapRepository, VictoriaTapRepository>();
builder.Services.AddScoped<ISpendingInfoCalculator, SpendingInfoCalculator>();
builder.Services.AddScoped<IVictoriaValidatorHandler, VictoriaValidatorHandler>();

builder.Services.AddTransient<IValidator<ChangeDispenserStatusRequest>, ChangeDispenserStatusValidator>();
builder.Services.AddTransient<IValidator<CreateDispenserRequest>, CreateDispenserValidator>();
builder.Services.AddTransient<IValidator<GetDispenserSpendingInfoRequest>, GetDispenserSpendingInfoValidator>();

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
