using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
    public class EducationInfoMapper : IEducationInfoMapper
    {
        public EducationInfo Map(DbUserEducation dbUserEducation)
        {
            if (dbUserEducation == null)
            {
                throw new ArgumentNullException(nameof(dbUserEducation));
            }

            return new EducationInfo
            {
                Id = dbUserEducation.Id,
                UniversityName = dbUserEducation.UniversityName,
                QualificationName = dbUserEducation.QualificationName,
                FormEducation = (FormEducation)dbUserEducation.FormEducation,
                AdmissiomAt = dbUserEducation.AdmissiomAt,
                IssueAt = dbUserEducation.IssueAt
            };
        }
    }
}
