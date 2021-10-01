using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using Microsoft.AspNetCore.Http;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
    public class DbUserCertificateMapper : IDbUserCertificateMapper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DbUserCertificateMapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbUserCertificate Map(CreateCertificateRequest request, Guid imageId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbUserCertificate
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ImageId = imageId,
                Name = request.Name,
                SchoolName = request.SchoolName,
                EducationType = (int)request.EducationType,
                ReceivedAt = request.ReceivedAt,
                IsActive = true,
                CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
                CreatedAtUtc = DateTime.UtcNow,
            };
        }
    }
}
