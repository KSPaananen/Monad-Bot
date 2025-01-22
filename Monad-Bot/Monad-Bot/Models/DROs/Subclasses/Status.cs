using System.Text.Json.Serialization;

namespace Monad_Bot.Models.DROs.Subclasses
{
    public class Status
    {
        [JsonPropertyName("card")]
        public int Card { get; set; }

        [JsonPropertyName("money")]
        public int Money { get; set; }

        [JsonPropertyName("players")]
        public List<Player> Players { get; set; }

        [JsonPropertyName("cardsLeft")]
        public int CardsLeft { get; set; }

        [JsonPropertyName("finished")]
        public bool Finished { get; set; }

    }
}
