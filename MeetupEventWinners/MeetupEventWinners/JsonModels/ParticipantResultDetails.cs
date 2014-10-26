using Newtonsoft.Json;

namespace MeetupEventWinners.JsonModels
{
    public class ParticipantResultDetails
    {
        [JsonProperty("results")]
        public Participant[] Participants { get; set; }
    }
}
