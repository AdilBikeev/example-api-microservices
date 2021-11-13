using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using System.Threading.Tasks;
using PlatformService.SyncDataServices.Http;
using PlatformService.AsyncDataServices;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataCLient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(
            IPlatformRepo repository, 
            IMapper mapper,
            ICommandDataClient commandDataCLient,
            IMessageBusClient messageBusClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataCLient = commandDataCLient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms....");

            var platformItem = _repository.GetAllPlatform();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItem));
        }


        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatformById(int id)
        {
            Console.WriteLine("--> Getting Platform by id....");
            var platformItem = _repository.GetPlatformById(id);
            if (platformItem is not null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }

            return NotFound();
        }


        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            Console.WriteLine("--> Create Platform....");
            var platfomrModel = _mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(platfomrModel);
            _repository.SaveChanges();

            var platformReadModel = _mapper.Map<PlatformReadDto>(platfomrModel);

            // Send Sync Message
            try
            {
                 await _commandDataCLient.SendPlatformToCommand(platformReadModel);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }

            // Send Async Message
            try
            {
                 var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadModel);
                 platformPublishedDto.Event = "Platfrom_Published";
                 _messageBusClient.PublishNewPlatform(platformPublishedDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send asynchronously: {ex.Message}");
            }

            var platformReadDto = _mapper.Map<PlatformReadDto>(platfomrModel);
            return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
        }
    }
}