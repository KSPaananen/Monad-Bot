namespace Monad_Bot.Services.Interfaces
{
    public interface IGameService
    {
        Task<bool> RunGamesAsync(int count);

    }
}
