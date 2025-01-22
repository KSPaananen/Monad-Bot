using Microsoft.Extensions.Configuration;
using Monad_Bot.Repositories.Interfaces;
using System.Reflection;

namespace Monad_Bot.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IConfiguration _configuration;

        public ConfigurationRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetPlayerName()
        {
            return _configuration.GetSection("Bot:Name").Value ?? throw new ArgumentException($"Bot:Name was null or empty at {this.GetType().Name} : {MethodBase.GetCurrentMethod()!.Name}");
        }

        public string GetBotToken()
        {
            return _configuration.GetSection("Bot:Token").Value ?? throw new ArgumentException($"Bot:Token was null or empty at {this.GetType().Name} : {MethodBase.GetCurrentMethod()!.Name}");
        }

        public string GetBaseUrl()
        {
            return _configuration.GetSection("Api:BaseUrl").Value ?? throw new ArgumentException($"Api:BaseUrl was null or empty at {this.GetType().Name} : {MethodBase.GetCurrentMethod()!.Name}");
        }


    }
}
