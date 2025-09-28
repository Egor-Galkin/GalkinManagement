using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagementWpfApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ManagementWpfApp.Services
{
    public class MissionService
    {
        private readonly ApplicationContext _context;

        public MissionService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<Mission>> GetMissionsForUserAsync(User user)
        {
            if (user.Role.Name == "Администратор")
            {
                return await _context.Missions
                    .Include(m => m.Author.Role)
                    .Include(m => m.Performer.Role)
                    .ToListAsync();
            }
            else if (user.Role.Name == "Руководитель")
            {
                return await _context.Missions
                    .Include(m => m.Author.Role)
                    .Include(m => m.Performer.Role)
                    .Where(m => m.AuthorId == user.UserId)
                    .ToListAsync();
            }
            else // Сотрудник
            {
                return await _context.Missions
                    .Include(m => m.Author.Role)
                    .Include(m => m.Performer.Role)
                    .Where(m => m.PerformerId == user.UserId)
                    .ToListAsync();
            }
        }

        public async Task SaveMissionAsync(Mission mission)
        {
            if (mission.MissionId == 0)
                _context.Missions.Add(mission);
            else
                _context.Entry(mission).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteMissionAsync(Mission mission)
        {
            _context.Missions.Remove(mission);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.Include(u => u.Role).ToListAsync();
        }
    }
}