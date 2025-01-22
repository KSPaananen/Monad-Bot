using Monad_Bot.Models.DROs;

namespace Monad_Bot.Services.Interfaces
{
    public interface IApiService
    {
        Task<GameInfo> StartGameAsync();

        Task<GameState> PerformActionAsync(string gameId, bool takeCard);

    }
}
