using Newtonsoft.Json;

namespace MeetupEventWinners.JsonModels
{
    public class Participant
    {
        [JsonProperty("member")]
        public Member Member { get; set; }
    }
}