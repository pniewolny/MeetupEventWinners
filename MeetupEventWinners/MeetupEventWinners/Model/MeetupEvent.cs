using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MeetupEventWinners.JsonModels;
using Newtonsoft.Json;
using RestSharp;

namespace MeetupEventWinners.Model
{
    internal class MeetupEvent
    {
        public MeetupSettings Settings { get; private set; }

        public MeetupEvent(MeetupSettings settings)
        {
            Settings = settings;
        }

        public void GetGroupDetails()
        {
            Contract.Assert(string.IsNullOrEmpty(Settings.MeetupApiKey) == false, "API Key is required!");
            Contract.Assert(string.IsNullOrEmpty(Settings.GroupName) == false, "Group Name is required!");

            Console.WriteLine("Reading group details...");
            var client = new RestClient(Settings.ApiUri);

            var request = new RestRequest("find/groups", Method.GET);
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("key", Settings.MeetupApiKey);
            request.AddParameter("text", Settings.GroupName);

            var response = client.Execute(request);
            var content = response.Content;

            var groups = JsonConvert.DeserializeObject<List<GroupDetail>>(content);

            if (groups.Any() == false)
            {
                Console.WriteLine("No groups found!");
                return;
            }
            if (groups.Count > 1)
            {
                Console.WriteLine("More than one group found with name: '" + Settings.GroupName + "'");
                return;
            }

            Settings.GroupId = groups[0].GroupId;

            Contract.Assert(groups.Any(), "No groups found!");
            Contract.Assert(string.IsNullOrEmpty(Settings.MeetupApiKey) == false, "API Key is required!");
        }

        public void GetCurrentEventDetails()
        {
            Contract.Assert(string.IsNullOrEmpty(Settings.MeetupApiKey) == false, "API Key is required!");
            Contract.Assert(string.IsNullOrEmpty(Settings.GroupId) == false, "Id of a group is required!");

            Console.WriteLine("Reading event details...");
            var client = new RestClient(Settings.ApiUri);

            var request = new RestRequest("/2/events", Method.GET);
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("key", Settings.MeetupApiKey);
            request.AddParameter("group_id", Settings.GroupId);
            request.AddParameter("time", string.Format("{0},{1}", Math.Round(Settings.EventDateBoundaryLeft, 0), Math.Round(Settings.EventDateBoundaryRight, 0)));

            var response = client.Execute(request);
            var content = response.Content;

            var jsonValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            var jsonResultsContent = jsonValues["results"].ToString();

            var events = JsonConvert.DeserializeObject<List<EventDetails>>(jsonResultsContent);
            if (events.Any() == false)
            {
                Console.WriteLine("No events found!");
                return;
            }
            if (events.Count > 1)
            {
                Console.WriteLine("More than one event found!");
                return;
            }

            Settings.MeetupEventDetails = events[0];

            Console.WriteLine();
            Console.WriteLine("Event name: '" + Settings.MeetupEventDetails.EventName + "'");
            Console.WriteLine("Participants with 'yes' RVSP: '" + Settings.MeetupEventDetails.RvspLimit + "'");
            Console.WriteLine();
        }

        public void GetParticipants()
        {
            Contract.Assert(string.IsNullOrEmpty(Settings.MeetupApiKey) == false, "API Key is required!");
            Contract.Assert(Settings.MeetupEventDetails != null, "Event details are unknown!");

            Console.WriteLine("Reading participants...");

            var client = new RestClient(Settings.ApiUri);

            var request = new RestRequest("/2/rsvps", Method.GET);
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("key", Settings.MeetupApiKey);
            request.AddParameter("event_id", Settings.MeetupEventDetails.EventId);
            request.AddParameter("rsvp", "yes");
            request.AddParameter("page", Settings.MeetupEventDetails.RvspLimit);

            var response = client.Execute(request);
            var content = response.Content;

            var jsonValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            var jsonResultsContent = jsonValues["results"].ToString();

            Settings.Participants = JsonConvert.DeserializeObject<List<Participant>>(jsonResultsContent);
        }

        public void PresentEventWinners()
        {
            Contract.Assert(Settings.Participants != null, "Participants are unknown!");

            Console.WriteLine();
            Console.WriteLine("Winners:");
            Console.WriteLine();

            var idx = 1;
            foreach (var member in Settings.Participants.Select(s => s.Member).Take(Settings.MaxWinners).OrderByDescending(p => p.UniqueId))
            {
                Console.WriteLine("{0}. {1}", idx, member.Name);
                idx++;
            }
        }

    }
}