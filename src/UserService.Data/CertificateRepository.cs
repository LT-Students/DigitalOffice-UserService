using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Data
{
  public class CertificateRepository : ICertificateRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CertificateRepository> _logger;

    public CertificateRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor,
      ILogger<CertificateRepository> logger)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
      _logger = logger;
    }

    public void Add(DbUserCertificate certificate)
    {
      if (certificate == null)
      {
        _logger.LogWarning(new ArgumentNullException(nameof(certificate)).Message);
        return;
      }

      _provider.UserCertificates.Add(certificate);
      _provider.Save();
    }

    public DbUserCertificate Get(Guid certificateId)
    {
      DbUserCertificate certificate = _provider.UserCertificates
        .FirstOrDefault(x => x.Id == certificateId);

      if (certificate == null)
      {
        _logger.LogWarning($"User certificate with ID '{certificateId}' was not found.");
      }

      return certificate;
    }

    public bool Edit(DbUserCertificate certificate, JsonPatchDocument<DbUserCertificate> request)
    {
      if (certificate == null)
      {
        _logger.LogWarning(new ArgumentNullException(nameof(certificate)).Message);
        return false;
      }

      if (request == null)
      {
        _logger.LogWarning(new ArgumentNullException(nameof(request)).Message);
        return false;
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
        _logger.LogWarning(new ArgumentNullException(nameof(certificate)).Message);
        return false;
      }

      _provider.UserCertificates.Remove(certificate);
      _provider.Save();

      return true;
    }
  }
}
