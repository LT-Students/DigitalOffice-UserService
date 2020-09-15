using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef;
using System;
using System.Linq;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.UserService.Data
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
        public UserRepository([FromServices] UserServiceDbContext userServiceDbContext)
        {
            this.userServiceDbContext = userServiceDbContext;
        }

        //TODO: Change exception type
        public Guid CreateUser(DbUser user)
        {
            if (userServiceDbContext.Users.Any(users => user.Email == users.Email)) throw new Exception("Email is already taken.");

            userServiceDbContext.Users.Add(user);
            userServiceDbContext.SaveChanges();

            return user.Id;
        }

        //TODO: Change exception type
        public DbUser GetUserInfoById(Guid userId)
        {
            return userServiceDbContext.Users.FirstOrDefault(dbUser => dbUser.Id == userId) ?? throw new Exception("User with this id not found.");
        }

        //TODO: Think about return type.
        public bool EditUser(DbUser user)
        {
            if (!userServiceDbContext.Users.Any(users => user.Id == users.Id)) throw new Exception("User was not found.");

            userServiceDbContext.Users.Update(user);
            userServiceDbContext.SaveChanges();

            return true;
        }

        //TODO: Change exception type
        public DbUser GetUserByEmail(string userEmail)
        {
            var user = userServiceDbContext.Users.FirstOrDefault(u => u.Email == userEmail) ?? throw new Exception("User not found.");

            return user;
        }
    }
}
