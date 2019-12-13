using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using TestProject.WebAPI.Data;

namespace TestProject.WebAPI.Services
{
    public class UsersService : IUsersService
    {
        private readonly TestProjectContext _testProjectContext;

        public UsersService(TestProjectContext testProjectContext)
        {
            _testProjectContext = testProjectContext;
        }

        public async Task<IEnumerable<User>> Get(int[] ids, Filters filters)
        {
            var users = _testProjectContext.Users.AsQueryable();

            if (filters == null)
                filters = new Filters();

            if (filters.Ages != null && filters.Ages.Any())
                users = users.Where(x => filters.Ages.Contains(x.Age));

            if (ids != null && ids.Any())
                users = users.Where(x => ids.Contains(x.Id));

            return await users.ToListAsync();
        }

        public async Task<User> Add(User user)
        {
            await _testProjectContext.Users.AddAsync(user);
            await _testProjectContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> Update(User user)
        {
            _testProjectContext.Users.Update(user);
            await _testProjectContext.SaveChangesAsync();
            return user;
        }

        public async Task<bool> Delete(User user)
        {
            _testProjectContext.Users.Remove(user);
            await _testProjectContext.SaveChangesAsync();

            return true;
        }
    }

    public interface IUsersService
    {
        Task<IEnumerable<User>> Get(int[] ids, Filters filters);

        Task<User> Add(User user);

        Task<User> Update(User user);

        Task<bool> Delete(User user);
    }

    public class Filters
    {
        public uint[] Ages { get; set; }
    }
}
