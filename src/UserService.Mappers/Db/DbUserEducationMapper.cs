using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
    public class DbUserEducationMapper : IDbUserEducationMapper
    {
        public DbUserEducation Map(CreateEducationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new DbUserEducation
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                UniversityName = request.UniversityName,
                QualificationName = request.QualificationName,
                AdmissionAt = request.AdmissionAt,
                IssueAt = request.IssueAt,
                FormEducation = (int)request.FormEducation,
                IsActive = true
            };
        }
    }
}
