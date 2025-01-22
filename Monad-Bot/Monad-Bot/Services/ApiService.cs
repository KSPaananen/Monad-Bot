using Microsoft.Extensions.Logging;
using Monad_Bot.Models.DROs;
using Monad_Bot.Repositories.Interfaces;
using Monad_Bot.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace Monad_Bot.Services
{
    // API docs:
    // - https://koodipahkina.monad.fi/app/docs

    public class ApiService : IApiService
    {
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ILogger<ApiService> _logger;

        private string _baseUrl;
        private int _retryDelay;
        private int _retryLimit;

        private HttpClient _client;

        public ApiService(IConfigurationRepository configurationRepository, ILogger<ApiService> logger, IHttpClientFactory clientFactory)
        {
            _configurationRepository = configurationRepository ?? throw new NullReferenceException(nameof(configurationRepository));
            _logger = logger ?? throw new NullReferenceException(nameof(logger));

            // Set/Read required values
            _baseUrl = _configurationRepository.GetBaseUrl();
            _retryDelay = 1000; // Delay in milliseconds
            _retryLimit = 5;

            // Create HttpClient with defaults
            string token = _configurationRepository.GetBotToken();

            _client = clientFactory.CreateClient();
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        public async Task<GameInfo> StartGameAsync()
        {
            int retryCount = 0;

            while (retryCount < _retryLimit)
            {
                try
                {
                    // Start a new game by sending a post request to /game
                    HttpResponseMessage response = await _client.PostAsync($"{_baseUrl}/game", null);

                    // Handle possible failed requests or bad responses
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"Request failed with code {response.StatusCode}");
                    }

                    string body = await response.Content.ReadAsStringAsync();

                    if (String.IsNullOrEmpty(body))
                    {
                        throw new InvalidOperationException($"Response body was empty");
                    }

                    var gameInfo = JsonSerializer.Deserialize<GameInfo>(body);

                    if (gameInfo == null)
                    {
                        throw new InvalidOperationException($"Unable to deserialize game info");
                    }

                    return await Task.FromResult(gameInfo);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning($"Http request failed with statuscode {ex.StatusCode}");

                    // Grow retryCount by 1 on http request failure
                    retryCount++;

                    // Delay attempts
                    await Task.Delay(_retryDelay);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical($"Unexpected error occured in StartGameAsync(). Exception: {ex.Message}");

                    throw;
                }
            }

            _logger.LogCritical($"Start game failed after {retryCount} attempts");

            throw new Exception($"Start game failed after {retryCount} attempts");
        }

        public async Task<GameState> PerformActionAsync(string gameId, bool takeCard)
        {
            int retryCount = 0;

            while (retryCount <= _retryLimit)
            {
                try
                {
                    // Create & serialize action object to StringContent
                    var action = new Models.DTOs.Action
                    {
                        TakeCard = takeCard
                    };

                    string jsonString = JsonSerializer.Serialize(action);

                    StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                    // Post the action to API. Response body will contain current game state
                    HttpResponseMessage response = await _client.PostAsync($"{_baseUrl}/game/{gameId}/action", content);

                    // Handle possible failed requests or bad responses
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new HttpRequestException($"Request failed with code {response.StatusCode}");
                    }

                    string body = await response.Content.ReadAsStringAsync();

                    if (String.IsNullOrEmpty(body))
                    {
                        throw new InvalidOperationException($"Response body was empty");
                    }

                    var gameState = JsonSerializer.Deserialize<GameState>(body);

                    if (gameState == null)
                    {
                        throw new InvalidOperationException($"Unable to deserialize game state");
                    }

                    return await Task.FromResult(gameState);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogWarning($"Http request failed with statuscode {ex.StatusCode}");

                    // Grow retryCount by 1 on http request failure
                    retryCount++;

                    // Delay attempts
                    await Task.Delay(_retryDelay);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical($"Unexpected error occured in PerformActionAsync(). Exception: {ex.Message}");

                    throw;
                }
            }

            _logger.LogCritical($"Perform action failed after {retryCount} attempts");

            throw new Exception($"Perform action failed after {retryCount} attempts");
        }


    }
}
