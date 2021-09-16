using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto;
using Microsoft.AspNetCore.Http;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
    public class DbSkillMapper : IDbSkillMapper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DbSkillMapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSkill Map(CreateSkillRequest request)
        {
            if (request == null)
            {
                return null;
            }

            return new DbSkill
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                IsActive = true,
                CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
                CreatedAtUtc = DateTime.UtcNow,
            };
        }
    }
}
