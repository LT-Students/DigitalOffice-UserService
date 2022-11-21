using DigitalOffice.Models.Broker.Publishing;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Publishing;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Company;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Department;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Image;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Office;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Position;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Right;
using LT.DigitalOffice.UserService.Broker.Publishes.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.UserCompany;
using MassTransit;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Publishes
{
  public class Publish : IPublish
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IBus _bus;

    public Publish(
      IHttpContextAccessor httpContextAccessor,
      IBus bus)
    {
      _httpContextAccessor = httpContextAccessor;
      _bus = bus;
    }

    public Task DisactivateUserAsync(Guid userId)
    {
      return _bus.Publish<IDisactivateUserPublish>(
        IDisactivateUserPublish.CreateObj(
          userId: userId,
          modifiedBy: _httpContextAccessor.HttpContext.GetUserId()));
    }

    public Task ActivateUserAsync(Guid userId)
    {
      return _bus.Publish<IActivateUserPublish>(
        IActivateUserPublish.CreateObj(userId: userId));
    }

    public Task CreatePendingUserAsync(Guid userId)
    {
      return _bus.Publish<ICreatePendingUserPublish>(
        ICreatePendingUserPublish.CreateObj(
          userId: userId,
          createdBy: _httpContextAccessor.HttpContext.GetUserId()));
    }

    public Task RemoveImagesAsync(List<Guid> imagesIds)
    {
      return imagesIds is null || !imagesIds.Any()
        ? Task.CompletedTask
        : _bus.Publish<IRemoveImagesPublish>(IRemoveImagesPublish.CreateObj(
          imagesIds: imagesIds,
          imageSource: ImageSource.User));
    }

    public Task CreateUserOfficeAsync(Guid userId, Guid officeId)
    {
      return _bus.Publish<ICreateUserOfficePublish>(
        ICreateUserOfficePublish.CreateObj(
          userId: userId,
          officeId: officeId,
          createdBy: _httpContextAccessor.HttpContext.GetUserId()));
    }

    public Task CreateUserRoleAsync(Guid userId, Guid roleId)
    {
      return _bus.Publish<ICreateUserRolePublish>(
        ICreateUserRolePublish.CreateObj(
          userId: userId,
          roleId: roleId,
          createdBy: _httpContextAccessor.HttpContext.GetUserId()));
    }

    public Task CreateDepartmentUserAsync(Guid userId, Guid departmentId)
    {
      return _bus.Publish<ICreateDepartmentUserPublish>(
        ICreateDepartmentUserPublish.CreateObj(
          userId: userId,
          departmentId: departmentId,
          createdBy: _httpContextAccessor.HttpContext.GetUserId(),
          isActive: false));
    }

    public Task CreateUserPositionAsync(Guid userId, Guid positionId)
    {
      return _bus.Publish<ICreateUserPositionPublish>(
        ICreateUserPositionPublish.CreateObj(
          userId: userId,
          positionId: positionId,
          createdBy: _httpContextAccessor.HttpContext.GetUserId()));
    }

    public Task CreateCompanyUserAsync(Guid userId, CreateUserCompanyRequest userCompany)
    {
      return _bus.Publish<ICreateCompanyUserPublish>(
        ICreateCompanyUserPublish.CreateObj(
          companyId: userCompany.CompanyId,
          userId: userId,
          contractSubjectId: userCompany.ContractSubjectId,
          contractTermType: userCompany.ContractTermType,
          rate: userCompany.Rate,
          startWorkingAt: userCompany.StartWorkingAt,
          endWorkingAt: userCompany.EndWorkingAt,
          probation: userCompany.Probation,
          createdBy: _httpContextAccessor.HttpContext.GetUserId()));
    }
  }
}
