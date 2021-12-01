using LT.DigitalOffice.Models.Broker.Models.Education;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class EducationInfoMapper : IEducationInfoMapper
  {
    public EducationInfo Map(EducationData educationData)
    {
      if (educationData is null)
      {
        return null;
      }

      return new EducationInfo
      {
        Id = educationData.Id,
        UniversityName = educationData.UniversityName,
        QualificationName = educationData.QualificationName,
        FormEducation = educationData.FormEducation,
        AdmissionAt = educationData.AdmissionAt,
        IssueAt = educationData.IssueAt
      };
    }
  }
}
