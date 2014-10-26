using Newtonsoft.Json;

namespace MeetupEventWinners.JsonModels
{
    public class EventDetails
    {
        [JsonProperty("id")]
        public string EventId { get; set; }

        [JsonProperty("name")]
        public string EventName { get; set; }

        [JsonProperty("rsvp_limit")]
        public int RvspLimit { get; set; }
    }
}
