using System;
using System.Collections.Generic;
using System.Linq;
using CommandService.Models;

namespace CommandService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly AppDbContext _context;

        public CommandRepo(AppDbContext context)
        {
            _context = context;
        }

        public void CreateCommand(int platformId, Command command)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.PlatformId = platformId;
            _context.Commands.Add(command);
        }

        public void CreatePlatform(Platform plat)
        {
            if (plat is null)
            {
                throw new ArgumentNullException(nameof(plat));
            }

            _context.Platforms.Add(plat);
        }

        public bool ExternalPlatformExists(int externalPlatformId)
        {
             return _context.Platforms.Any(p => p.ExternalId == externalPlatformId);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public Command GetCommand(int platfromId, int commandId)
        {
            return _context.Commands
                .Where(c => c.PlatformId == platfromId && c.Id == commandId).FirstOrDefault();
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return _context.Commands
            .Where(c => c.PlatformId == platformId)
            .OrderBy(c => c.Platform.Name);
        }

        public bool PlatformExist(int platformId)
        {
            return _context.Platforms.Any(p => p.Id == platformId);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}