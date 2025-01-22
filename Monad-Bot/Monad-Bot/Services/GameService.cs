using Monad_Bot.Models.DROs;
using Monad_Bot.Models.DROs.Subclasses;
using Monad_Bot.Models.Game;
using Monad_Bot.Repositories.Interfaces;
using Monad_Bot.Services.Interfaces;

namespace Monad_Bot.Services
{
    public class GameService : IGameService
    {
        private readonly IConfigurationRepository _configurationRepository;

        private readonly IApiService _apiService;

        private string _playerName;

        public GameService(IConfigurationRepository configurationRepository, IApiService apiService)
        {
            _configurationRepository = configurationRepository ?? throw new NullReferenceException(nameof(configurationRepository));

            _apiService = apiService ?? throw new NullReferenceException(nameof(apiService));

            _playerName = _configurationRepository.GetPlayerName();
        }

        public async Task<bool> RunGamesAsync(int count)
        {
            try
            {
                for (int i = 0; i < count; i++)
                {
                    // Start a new game. gameInfo will contain games id & status
                    Console.WriteLine($"Starting game number {i + 1}");

                    GameInfo gameInfo = await StartGameAsync();

                    var status = gameInfo.Status;
                    var gameId = gameInfo.GameId;

                    Console.WriteLine($"Playing game number {i + 1}...");

                    // Run turn handling in a loop until Finished property is set to true
                    do
                    {
                        status = await HandleTurn(gameId, status);
                    }
                    while (!status.Finished);

                    // Display results in console
                    DisplayResults(status);
                }

                // Return true after everything was executed succesfully
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<GameInfo> StartGameAsync()
        {
            GameInfo gameInfo = await _apiService.StartGameAsync();

            return gameInfo;
        }

        private async Task<Status> HandleTurn(string gameId, Status status)
        {
            int maxCardValue = 16; // Card value - coins on the table value that the bot should consider taking

            // Find bots player data from the playerlist
            Player bot = status.Players.Where(p => p.Name == _playerName).First();

            // Take card if bot is out of money
            if (bot.Money <= 0)
            {
                GameState gameState = await _apiService.PerformActionAsync(gameId, true);

                return gameState.Status;
            }

            var tableCard = status.Card;
            var tableMoney = status.Money;

            // Bet if theres less than 4 coins
            if (tableMoney < 4)
            {
                GameState gameState = await _apiService.PerformActionAsync(gameId, false);

                return gameState.Status;
            }

            // Take a card if the bot doesn't have a set amount of card rows.
            // Prevent taking high score cards with maxCardValue
            if (bot.Cards.Count < 4 && tableCard - tableMoney <= maxCardValue)
            {
                GameState gameState = await _apiService.PerformActionAsync(gameId, true);

                return gameState.Status;
            }

            // Check if bot has a CardsList containing adjecent numbers to card on the table
            // Take the card if this is true
            foreach (var cardList in bot.Cards)
            {
                if (cardList.Contains(tableCard - 1) || cardList.Contains(tableCard + 1))
                {
                    GameState gameState = await _apiService.PerformActionAsync(gameId, true);

                    return gameState.Status;
                }
            }

            // Calculate the substraction of cards number & money on the table
            // Take if result is small enough
            if (tableCard - tableMoney <= maxCardValue)
            {
                GameState gameState = await _apiService.PerformActionAsync(gameId, true);

                return gameState.Status;
            }
            else
            {
                GameState gameState = await _apiService.PerformActionAsync(gameId, false);

                return gameState.Status;
            }
        }

        private void DisplayResults(Status status)
        {
            // Calculate the score for each player and add it to results list
            List<ScoreboardPlayer> resultList = new List<ScoreboardPlayer>();

            foreach (var player in status.Players)
            {
                int score = 0;

                foreach (var cardList in player.Cards)
                {
                    cardList.Order();

                    score += cardList.FirstOrDefault();
                }

                var result = new ScoreboardPlayer
                {
                    Name = player.Name,
                    Score = score - player.Money
                };

                resultList.Add(result);
            }

            resultList = resultList.OrderBy(p => p.Score).ToList();

            // Print results to console
            Console.WriteLine("-- Match results --");

            for (int i = 0; i < resultList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {resultList[i].Name}");
                Console.WriteLine($"Score: {resultList[i].Score}");

            }
        }


    }
}
