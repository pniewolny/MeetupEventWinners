using System;
using Newtonsoft.Json;

namespace MeetupEventWinners.JsonModels
{
    public class Member
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        public Guid UniqueId
        {
            get
            {
                {
                    return Guid.NewGuid();
                }
            }
        }
    }
}