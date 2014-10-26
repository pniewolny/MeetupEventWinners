using Newtonsoft.Json;

namespace MeetupEventWinners.JsonModels
{
    public class GroupDetail
    {
        [JsonProperty("id")]
        public string GroupId { get; set; }
    }
}
