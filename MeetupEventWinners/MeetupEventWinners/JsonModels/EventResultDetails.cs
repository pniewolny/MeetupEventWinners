﻿using Newtonsoft.Json;
using RestSharp;

namespace MeetupEventWinners.JsonModels
{
    public class EventResultDetails
    {
        [JsonProperty("results")]
        public EventDetails[] Events { get; set; }
    }
}
