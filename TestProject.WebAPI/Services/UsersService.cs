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

            if (filters.FirstNames != null && filters.FirstNames.Any())
                users = users.Where(x => filters.FirstNames.Contains(x.FirstName));

            if (filters.LastNames != null && filters.LastNames.Any())
                users = users.Where(x => filters.LastNames.Contains(x.FirstName));

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

        public async Task<IEnumerable<User>> AddRange(IEnumerable<User> users)
        {
            await _testProjectContext.Users.AddRangeAsync(users);
            await _testProjectContext.SaveChangesAsync();
            return users;
        }

        public async Task<User> Update(User user)
        {
            var userForChanges = await _testProjectContext.Users.SingleAsync(x => x.Id == user.Id);
            userForChanges.Age = user.Age;
            userForChanges.Password = user.Password;
            userForChanges.Email = user.Email;
            userForChanges.FirstName = user.FirstName;
            userForChanges.LastName = user.LastName;

            _testProjectContext.Users.Update(userForChanges);
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

        Task<IEnumerable<User>> AddRange(IEnumerable<User> users);

        Task<User> Update(User user);

        Task<bool> Delete(User user);
    }

    public class Filters
    {
        public uint[] Ages { get; set; }
        public string[] FirstNames { get; set; }
        public string[] LastNames { get; set; }
    }
}
