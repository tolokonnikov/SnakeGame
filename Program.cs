namespace SnakeGame;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SnakeGame.Application.Services;
using SnakeGame.Domain.Entities;
using SnakeGame.Domain.Services;
using SnakeGame.Infrastructure.Rendering;
using SnakeGame.Presentation;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using var scope = host.Services.CreateScope();
        var app = scope.ServiceProvider.GetRequiredService<GameApplication>();

        await app.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Domain Services
                services.AddSingleton<IFoodGenerator, RandomFoodGenerator>();

                // Application Services
                services.AddScoped<IGameEngine>(provider =>
                    new GameEngine(
                        provider.GetRequiredService<IFoodGenerator>(),
                        new GameSettings(Width: 25, Height: 20, InitialSpeed: 150)
                    ));

                // Infrastructure Services
                services.AddSingleton<IGameRenderer, ConsoleGameRenderer>();

                // Presentation Services
                services.AddScoped<GameApplication>();
            });
}