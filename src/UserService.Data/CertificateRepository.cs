using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Data
{
  public class CertificateRepository : ICertificateRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CertificateRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public void Add(DbUserCertificate certificate)
    {
      if (certificate == null)
      {
        throw new ArgumentNullException(nameof(certificate));
      }

      ICollection<DbUserCertificateImage> images = certificate.Images;

      if (images.Any())
      {
        _provider.UserCertificateImages.AddRange(images);
      }

      _provider.UserCertificates.Add(certificate);
      _provider.Save();
    }

    public DbUserCertificate Get(Guid certificateId)
    {
      DbUserCertificate certificate = _provider.UserCertificates
        .Include(c => c.Images)
        .FirstOrDefault(x => x.Id == certificateId);

      if (certificate == null)
      {
        throw new NotFoundException($"User certificate with ID '{certificateId}' was not found.");
      }

      return certificate;
    }

    public bool Edit(DbUserCertificate certificate, JsonPatchDocument<DbUserCertificate> request)
    {
      if (certificate == null)
      {
        throw new ArgumentNullException(nameof(certificate));
      }

      if (request == null)
      {
        throw new ArgumentNullException(nameof(request));
      }

      request.ApplyTo(certificate);
      certificate.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      certificate.ModifiedAtUtc = DateTime.UtcNow;
      _provider.Save();

      return true;
    }

    public bool Remove(DbUserCertificate certificate)
    {
      if (certificate == null)
      {
        throw new ArgumentNullException(nameof(certificate));
      }

      _provider.UserCertificateImages.RemoveRange(certificate.Images);
      _provider.UserCertificates.Remove(certificate);
      _provider.Save();

      return true;
    }
  }
}
