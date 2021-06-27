using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace LT.DigitalOffice.UserService.Data
{
    /// <inheritdoc />
    public class UserCredentialsRepository : IUserCredentialsRepository
    {
        private readonly HttpContext _httpContext;
        private readonly ILogger<UserCredentialsRepository> _logger;
        private readonly IDataProvider _provider;

        public UserCredentialsRepository(
            ILogger<UserCredentialsRepository> logger,
            IHttpContextAccessor httpContextAccessor,
            IDataProvider provider)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _logger = logger;
            _provider = provider;
        }

        public DbUserCredentials Get(GetCredentialsFilter filter)
        {
            DbUserCredentials dbUserCredentials = null;
            if (filter.UserId.HasValue)
            {
                dbUserCredentials = _provider.UserCredentials.FirstOrDefault(
                    uc =>
                        uc.UserId == filter.UserId.Value);
            }
            else if (!string.IsNullOrEmpty(filter.Login))
            {
                dbUserCredentials = _provider.UserCredentials.FirstOrDefault(
                    uc =>
                        uc.Login == filter.Login);
            }
            else if (!string.IsNullOrEmpty(filter.Email) || !string.IsNullOrEmpty(filter.Phone))
            {
                dbUserCredentials = _provider.UserCredentials
                    .Include(uc => uc.User)
                        .ThenInclude(u => u.Communications)
                    .FirstOrDefault(
                        uc =>
                            uc.User.Communications.Any(
                                c =>
                                    (c.Type == (int)CommunicationType.Email &&
                                     c.Value == filter.Email) ||
                                    (c.Type == (int)CommunicationType.Phone &&
                                     c.Value == filter.Phone)));
            }
            else
            {
                throw new BadRequestException("You must specify 'userId' or 'login'.");
            }

            if (dbUserCredentials == null)
            {
                _logger.LogWarning($"Can not find user credentials filter '{filter}'.");

                throw new NotFoundException($"User credentials was not found.");
            }

            return dbUserCredentials;
        }

        public bool Edit(DbUserCredentials userCredentials)
        {
            if (userCredentials == null)
            {
                throw new ArgumentNullException(nameof(userCredentials));
            }

            _logger.LogInformation(
                $"Updating user credentials for user '{userCredentials.UserId}'. Request came from IP '{_httpContext.Connection.RemoteIpAddress}'.");

            if (!_provider.UserCredentials.Any(uc => uc.UserId == userCredentials.UserId))
            {
                throw new NotFoundException("User credentials was not found.");
            }

            try
            {
                _provider.UserCredentials.Update(userCredentials);
                _provider.Save();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Failed to update user credentials for user '{userCredentials.UserId}'. Request came from IP '{_httpContext.Connection.RemoteIpAddress}'.");

                return false;
            }

            return true;
        }

        public Guid Create(DbUserCredentials dbUserCredentials)
        {
            _provider.UserCredentials.Add(dbUserCredentials);
            _provider.Save();

            return dbUserCredentials.Id;
        }

        public void CheckLogin(string login, Guid userId)
        {
            if (_provider.UserCredentials.Any(
                uc => uc.Login == login))
            {
                throw new BadRequestException("Login is busy.");
            }

            if (_provider.UserCredentials.Any(
                uc => uc.UserId == userId))
            {
                throw new BadRequestException("User credentials is exist");
            }
        }
    }
}
