namespace Monad_Bot.Repositories.Interfaces
{
    public interface IConfigurationRepository
    {
        string GetBotToken();

        string GetPlayerName();

        string GetBaseUrl();

    }
}
