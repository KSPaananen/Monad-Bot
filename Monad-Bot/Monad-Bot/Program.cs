using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Monad_Bot.Repositories;
using Monad_Bot.Repositories.Interfaces;
using Monad_Bot.Services;
using Monad_Bot.Services.Interfaces;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", false, true);

// Add & configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

// Repositories
builder.Services.AddTransient<IConfigurationRepository, ConfigurationRepository>();

// Services
builder.Services.AddHostedService<MonadBotHostedService>();
builder.Services.AddTransient<IGameService, GameService>();
builder.Services.AddTransient<IApiService, ApiService>();

// Tools
builder.Services.AddHttpClient();

var app = builder.Build();

await app.RunAsync();