using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Linq;

namespace LT.DigitalOffice.UserService.Data
{
    public class EducationRepository : IEducationRepository
    {
        private readonly IDataProvider _provider;

        public EducationRepository(IDataProvider provider)
        {
            _provider = provider;
        }

        public void Add(DbUserEducation education)
        {
            if (education == null)
            {
                throw new ArgumentNullException(nameof(education));
            }

            _provider.UserEducations.Add(education);
            _provider.Save();
        }

        public DbUserEducation Get(Guid educationId)
        {
            DbUserEducation education = _provider.UserEducations.FirstOrDefault(e => e.Id == educationId);

            if (education == null)
            {
                throw new NotFoundException($"User education with ID '{educationId}' was not found.");
            }

            return education;
        }

        public bool Edit(DbUserEducation education, JsonPatchDocument<DbUserEducation> request)
        {
            if (education == null)
            {
                throw new ArgumentNullException(nameof(education));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            request.ApplyTo(education);
            _provider.Save();

            return true;
        }

        public bool Remove(DbUserEducation education)
        {
            if (education == null)
            {
                throw new ArgumentNullException(nameof(education));
            }

            education.IsActive = false;
            _provider.Save();

            return true;
        }
    }
}
