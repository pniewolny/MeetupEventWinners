using System.Collections.Generic;
using Newtonsoft.Json;

namespace MeetupEventWinners.JsonModels
{
    public class EventResultDetails
    {
        [JsonProperty("results")]
        public List<EventDetails> Events { get; set; }
    }
}
