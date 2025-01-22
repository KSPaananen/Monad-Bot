using System.Text.Json.Serialization;

namespace Monad_Bot.Models.DTOs
{
    public class Action
    {
        [JsonPropertyName("takeCard")]
        public bool TakeCard { get; set; }


    }
}
