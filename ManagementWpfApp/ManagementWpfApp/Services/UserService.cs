using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManagementWpfApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ManagementWpfApp.Services
{
    public class UserService
    {
        private readonly ApplicationContext _context;

        public UserService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.Include(u => u.Role).ToListAsync();
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task SaveUserAsync(User user)
        {
            if (user.UserId == 0)
                _context.Users.Add(user);
            else
                _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> AuthenticateAsync(string login, string password)
        {
            return await _context.Users.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
        }
    }
}