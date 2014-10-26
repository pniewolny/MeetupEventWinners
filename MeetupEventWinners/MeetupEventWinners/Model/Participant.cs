using Newtonsoft.Json;

namespace MeetupEventWinners.Model
{
    public class Participant
    {
        [JsonProperty("member")]
        public Member Member { get; set; }
    }
}