using System.Runtime.Serialization.Json;
using System;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using CommandService.Dtos;
using CommandService.Data;
using CommandService.Models;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining Event");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType.Event)
            {
                case "Platfrom_Published":
                    Console.WriteLine("--> Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could bot determine the event type");
                    return EventType.Underterminade;
            }
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);  
                    break;
                default:
                    break;
            }
        }

        private void AddPlatform(string platformPublishedMessage) {
            using (var scope = _scopeFactory.CreateScope())
            {
                 var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                 var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

                 try
                 {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repo.ExternalPlatformExists(plat.ExternalId))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                        Console.WriteLine("--> Platform added");
                    } else {
                        Console.WriteLine("--> Platform already exists");
                    }
                 }
                 catch (Exception ex)
                 {
                     Console.WriteLine($"--> Could not add Platform to DB {ex.Message}");
                 }
            }
        }

        enum EventType
        {
            PlatformPublished,
            Underterminade
        }
    }
}