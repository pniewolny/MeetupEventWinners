using System;
using System.Collections.Generic;
using MeetupEventWinners.JsonModels;

namespace MeetupEventWinners.Model
{
    internal class MeetupSettings
    {
        private readonly DateTime _unixMinTime = new DateTime(1970, 1, 1, 0, 0, 0);
        
        /// <summary>
        /// Maximal number of winners to present
        /// </summary>
        public int MaxWinners { get; set; }
        
        /// <summary>
        /// API Key from https://secure.meetup.com/meetup_api/key/
        /// </summary>
        public string MeetupApiKey { get; set; }

        /// <summary>
        /// Full name of the group in which we will search for events
        /// </summary>
        public string GroupName { get; set; }

        public string GroupId { get; internal set; }

        /// <summary>
        /// Date when event take place (optional)
        /// </summary>
        public DateTime? TimeOfEvent { get; set; }

        public EventDetails MeetupEventDetails { get; set; }

        public List<Participant> Participants { get; set; }

        public string ApiUri
        {
            get { return "https://api.meetup.com/"; }
        }

        public double EventDateBoundaryLeft
        {
            get
            {
                return TimeOfEvent.HasValue
                           ? (TimeOfEvent.Value.Date - _unixMinTime).TotalMilliseconds
                           : (DateTime.Today.Date - _unixMinTime).TotalMilliseconds;
            }
        }

        public double EventDateBoundaryRight
        {
            get
            {
                return TimeOfEvent.HasValue
                           ? (TimeOfEvent.Value.Date.AddDays(1) - _unixMinTime).TotalMilliseconds
                           : (DateTime.Today.Date.AddDays(1) - _unixMinTime).TotalMilliseconds;
            }
        }
    }
}