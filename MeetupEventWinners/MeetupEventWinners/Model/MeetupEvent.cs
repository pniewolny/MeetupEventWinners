using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
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

            var jsonValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(content.Substring(1, content.Length - 2));
            Settings.GroupId = jsonValues["id"].ToString();
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

            var eventObject = new
            {
                id = string.Empty,
                rsvp_limit = 0,
                name = string.Empty
            };

            var deserializedDetails = JsonConvert.DeserializeAnonymousType(jsonResultsContent.Substring(1, jsonResultsContent.Length - 2), eventObject);
            Settings.EventDetails = new EventDetails
            {
                EventId = deserializedDetails.id,
                EventName = deserializedDetails.name,
                MaxRvspNo = deserializedDetails.rsvp_limit
            };

            Console.WriteLine();
            Console.WriteLine("Event name: '" + Settings.EventDetails.EventName + "'");
            Console.WriteLine("Participants with 'yes' RVSP: '" + Settings.EventDetails.MaxRvspNo + "'");
            Console.WriteLine();
        }

        public void GetParticipants()
        {
            Contract.Assert(string.IsNullOrEmpty(Settings.MeetupApiKey) == false, "API Key is required!");
            Contract.Assert(Settings.EventDetails != null, "Event details are unknown!");

            Console.WriteLine("Reading participants...");

            var client = new RestClient(Settings.ApiUri);

            var request = new RestRequest("/2/rsvps", Method.GET);
            request.RequestFormat = DataFormat.Json;

            request.AddParameter("key", Settings.MeetupApiKey);
            request.AddParameter("event_id", Settings.EventDetails.EventId);
            request.AddParameter("rsvp", "yes");
            request.AddParameter("page", Settings.EventDetails.MaxRvspNo);

            var response = client.Execute(request);
            var content = response.Content;

            var jsonValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(content);
            var jsonResultsContent = jsonValues["results"].ToString();
            var fixedJsonResultsContent = jsonResultsContent.Substring(1, jsonResultsContent.Length - 2);

            var splitedContents = fixedJsonResultsContent.Split(new[] { "},\r\n  {" }, StringSplitOptions.RemoveEmptyEntries);

            var idx = 0;
            var participants = new List<Participant>();
            foreach (var line in splitedContents)
            {
                string completeLine;
                if (idx == 0)
                {
                    completeLine = line + "}";
                }
                else if (idx == Settings.EventDetails.MaxRvspNo - 1)
                {
                    completeLine = "{" + line;
                }
                else
                {
                    completeLine = "{" + line + "}";
                }

                var user = JsonConvert.DeserializeObject<Participant>(completeLine);
                participants.Add(user);

                idx++;
            }

            Settings.Participants = participants;
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