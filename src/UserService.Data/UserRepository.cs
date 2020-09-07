using System;
using System.Linq;
using UserService.Data.Interfaces;
using UserService.Data.Provider.MsSql.Ef;
using UserService.Models.Db;

namespace UserService.Data
{
    /// <summary>
    /// Represents interface of repository. Provides method for getting user model from database.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly UserServiceDbContext userServiceDbContext;

        /// <summary>
        /// Initialize new instance of <see cref="UserRepository"/> with specified <see cref="UserServiceDbContext"/> and <see cref="IMapper{TIn,TOut}"/>
        /// </summary>
        /// <param name="userServiceDbContext">Specified <see cref="userServiceDbContext"/></param>
        public UserRepository(UserServiceDbContext userServiceDbContext)
        {
            this.userServiceDbContext = userServiceDbContext;
        }

        public Guid UserCreate(DbUser user)
        {
            if (userServiceDbContext.Users.Any(users => user.Email == users.Email))
            {
                throw new Exception("Email is already taken.");
            }

            userServiceDbContext.Users.Add(user);
            userServiceDbContext.SaveChanges();

            return user.Id;
        }

        public DbUser GetUserInfoById(Guid userId)
            => userServiceDbContext.Users.FirstOrDefault(dbUser => dbUser.Id == userId) ??
               throw new Exception("User with this id not found.");

        public bool EditUser(DbUser user)
        {
            if (!userServiceDbContext.Users.Any(users => user.Id == users.Id))
            {
                throw new Exception("User was not found.");
            }

            userServiceDbContext.Users.Update(user);
            userServiceDbContext.SaveChanges();

            return true;
        }

        public DbUser GetUserByEmail(string userEmail)
        {
            DbUser user = userServiceDbContext.Users.FirstOrDefault(u => u.Email == userEmail);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return user;
        }
    }
}
