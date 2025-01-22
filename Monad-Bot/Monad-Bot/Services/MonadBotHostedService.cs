using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Monad_Bot.Services.Interfaces;

namespace Monad_Bot.Services
{
    public class MonadBotHostedService : IMonadBotHostedService, IHostedService
    {
        private readonly ILogger<MonadBotHostedService> _logger;

        private readonly IGameService _gameService;

        public MonadBotHostedService(ILogger<MonadBotHostedService> logging, IGameService gameService)
        {
            _logger = logging;

            _gameService = gameService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Starting bot...");

                await Task.Run(async () =>
                {
                    await InitializeBot();

                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning($"Operation cancelled...");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Stopping bot...");

            return Task.CompletedTask;
        }

        private async Task InitializeBot()
        {
            try
            {
                // Get the number of games to run and start playing
                Console.WriteLine("How many games would you like to play? Type the number of games which you'd like to play or type EXIT to exit program.");

                string? userInput = Console.ReadLine();

                // Keep prompting for another game until user types a non-numerical value
                while (!String.IsNullOrEmpty(userInput) && int.TryParse(userInput, out int count))
                {
                    await _gameService.RunGamesAsync(count);

                    Console.WriteLine("How many games would you like to play? Type the number of games which you'd like to play or type EXIT to exit program.");

                    userInput = Console.ReadLine();
                }

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
        }


    }
}
