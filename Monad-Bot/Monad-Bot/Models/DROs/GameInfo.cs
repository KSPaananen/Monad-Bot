using Monad_Bot.Models.DROs.Subclasses;
using System.Text.Json.Serialization;

namespace Monad_Bot.Models.DROs
{
    public class GameInfo
    {
        [JsonPropertyName("gameId")]
        public string GameId { get; set; }

        [JsonPropertyName("status")]
        public Status Status { get; set; }

    }
}
