using System.Text.Json.Serialization;

namespace Monad_Bot.Models.DROs.Subclasses
{
    public class Player
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("money")]
        public int Money { get; set; }

        [JsonPropertyName("cards")]
        public List<List<int>> Cards { get; set; }

    }
}
