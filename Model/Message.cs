using DrMarioPlayer.Converter;
using Newtonsoft.Json;

namespace DrMarioPlayer.Model
{
    [JsonConverter(typeof(JsonPathConverter))]
    internal class Message
    {
        [JsonProperty("Player.Coordinates")]
        public string[,] Coordinates { get; set; }

        [JsonProperty("Player.Score")]
        public int Score { get; set; }

        [JsonProperty("Player.Status")]
        public string Status { get; set; }

        [JsonProperty("Player.VirusRemaining")]
        public int VirusRemaining { get; set; }

        [JsonProperty("Player.ActivePill")]
        public string ActivePill { get; set; }

        [JsonProperty("Player.NextPill")]
        public string NextPill { get; set; }
    }
}
