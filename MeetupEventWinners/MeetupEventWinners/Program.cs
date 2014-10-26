using System;
using MeetupEventWinners.Model;

namespace MeetupEventWinners
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new MeetupSettings
            {
                MeetupApiKey = string.Empty, 
                GroupName = string.Empty, 
                MaxWinners = 10,
                TimeOfEvent = null
            };

            try
            {
                var meetupEvent = new MeetupEvent(settings);
                meetupEvent.GetGroupDetails();
                meetupEvent.GetCurrentEventDetails();
                meetupEvent.GetParticipants();
                meetupEvent.PresentEventWinners();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured: " + ex);
            }

            Console.ReadLine();
        }
    }
}
