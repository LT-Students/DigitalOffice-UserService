using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Models.Company;
using LT.DigitalOffice.Models.Broker.Models.Department;
using LT.DigitalOffice.Models.Broker.Models.Office;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Models.Right;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.User;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.UnitTests.User
{
  class GetUserCommandTests
  {
    private AutoMocker _mocker;

    private GetUserFilter _filter;
    private DbUser _dbUser;
    private UserResponse _userResponse;
    private OperationResultResponse<UserResponse> _response;
    private OperationResultResponse<UserResponse> _failureResponse;
    private IGetUserCommand _command;

    private List<CompanyData> _companiesData = new();
    private List<DepartmentData> _departmentsData = new();
    private List<ImageInfo> _imagesInfo = new();
    private List<OfficeData> _officesData = new();
    private List<PositionData> _positionsData = new();
    private List<RoleData> _rolesData = new();

    private CompanyUserInfo _companyUser = new();
    private DepartmentUserInfo _departmentUser = new();
    private OfficeInfo _officeUser = new();
    private PositionInfo _positionUser = new();
    private RoleInfo _userRole = new();

    private void Verifiable(
      Times responseCreatorCalls,
      Times userRepositoryCalls,
      Times companyServiceCalls,
      Times departmentServiceCalls,
      Times imageServiceCalls,
      Times officeServiceCalls,
      Times positionServiceCalls,
      Times rightServiceCalls,
      Times companyUserInfoMapperCalls,
      Times departmentInfoMapperCalls,
      Times officeInfoMapperCalls,
      Times positionInfoMapperCalls,
      Times roleInfoMapperCalls,
      Times userResponseMapperCalls)
    {
      _mocker.Verify<IResponseCreator>(
        x => x.CreateFailureResponse<UserResponse>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()),
        responseCreatorCalls);

      _mocker.Verify<IUserRepository>(
        x => x.GetAsync(It.IsAny<GetUserFilter>()),
        userRepositoryCalls);

      _mocker.Verify<ICompanyService>(
        x => x.GetCompaniesAsync(It.IsAny<Guid>(), It.IsAny<List<string>>(), default),
        companyServiceCalls);

      _mocker.Verify<IDepartmentService>(
        x => x.GetDepartmentsAsync(It.IsAny<Guid>(), It.IsAny<List<string>>(), default),
        departmentServiceCalls);

      _mocker.Verify<IImageService>(
        x => x.GetImagesAsync(It.IsAny<List<Guid>>(), It.IsAny<List<string>>(), default),
        imageServiceCalls);

      _mocker.Verify<IOfficeService>(
        x => x.GetOfficesAsync(It.IsAny<Guid>(), It.IsAny<List<string>>(), default),
        officeServiceCalls);

      _mocker.Verify<IPositionService>(
        x => x.GetPositionsAsync(It.IsAny<Guid>(), It.IsAny<List<string>>(), default),
        positionServiceCalls);

      _mocker.Verify<IRightService>(
        x => x.GetRolesAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<string>>(), default),
        rightServiceCalls);

      _mocker.Verify<ICompanyUserInfoMapper>(
        x => x.Map(It.IsAny<CompanyData>(), It.IsAny<CompanyUserData>()),
        companyUserInfoMapperCalls);

      _mocker.Verify<IDepartmentInfoMapper>(
        x => x.Map(It.IsAny<Guid>(), It.IsAny<DepartmentData>()),
        departmentInfoMapperCalls);

      _mocker.Verify<IOfficeInfoMapper>(
        x => x.Map(It.IsAny<OfficeData>()),
        officeInfoMapperCalls);

      _mocker.Verify<IPositionInfoMapper>(
        x => x.Map(It.IsAny<PositionData>()),
        positionInfoMapperCalls);

      _mocker.Verify<IRoleInfoMapper>(
        x => x.Map(It.IsAny<RoleData>()),
        roleInfoMapperCalls);

      _mocker.Verify<IUserResponseMapper>(
        x => x.Map(
          It.IsAny<DbUser>(),
          It.IsAny<CompanyUserInfo>(),
          It.IsAny<ImageInfo>(),
          It.IsAny<DepartmentUserInfo>(),
          It.IsAny<List<ImageInfo>>(),
          It.IsAny<OfficeInfo>(),
          It.IsAny<PositionInfo>(),
          It.IsAny<RoleInfo>()),
        userResponseMapperCalls);

      _mocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _failureResponse = new OperationResultResponse<UserResponse>(body: null, errors: new List<string>() { "Error" });
    }

    [SetUp]
    public void SetUp()
    {
      _filter = new GetUserFilter
      {
        UserId = Guid.NewGuid(),
        IncludeCurrentAvatar = true,
        IncludeAvatars = true,
        IncludeCommunications = true,
        IncludeCompany = true,
        IncludeDepartment = true,
        IncludeOffice = true,
        IncludePosition = true,
        IncludeRole = true,
        Locale = "en"
      };

      _dbUser = new DbUser()
      {
        Id = _filter.UserId.Value
      };

      _userResponse = new UserResponse()
      {
        User = new UserInfo(),
        UserAddition = new UserAdditionInfo(),
        CompanyUser = _companyUser,
        DepartmentUser = _departmentUser,
        Images = _imagesInfo,
        Office = _officeUser,
        Position = _positionUser,
        Role = _userRole
      };

      _response = new OperationResultResponse<UserResponse>(body: _userResponse);

      _mocker = new AutoMocker();

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<UserResponse>>(x => x.CreateFailureResponse<UserResponse>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()))
        .Returns(_failureResponse);

      _mocker
        .Setup<IUserRepository, Task<DbUser>>(x => x.GetAsync(It.IsAny<GetUserFilter>()))
        .Returns(Task.FromResult(_dbUser));

      _mocker
        .Setup<ICompanyService, Task<List<CompanyData>>>(x => x.GetCompaniesAsync(It.IsAny<Guid>(), It.IsAny<List<string>>(), default))
        .Returns(Task.FromResult(_companiesData));

      _mocker
        .Setup<IDepartmentService, Task<List<DepartmentData>>>(x => x.GetDepartmentsAsync(It.IsAny<Guid>(), It.IsAny<List<string>>(), default))
        .Returns(Task.FromResult(_departmentsData));

      _mocker
        .Setup<IImageService, Task<List<ImageInfo>>>(x => x.GetImagesAsync(It.IsAny<List<Guid>>(), It.IsAny<List<string>>(), default))
        .Returns(Task.FromResult(_imagesInfo));

      _mocker
        .Setup<IOfficeService, Task<List<OfficeData>>>(x => x.GetOfficesAsync(It.IsAny<Guid>(), It.IsAny<List<string>>(), default))
        .Returns(Task.FromResult(_officesData));

      _mocker
        .Setup<IPositionService, Task<List<PositionData>>>(x => x.GetPositionsAsync(It.IsAny<Guid>(), It.IsAny<List<string>>(), default))
        .Returns(Task.FromResult(_positionsData));

      _mocker
        .Setup<IRightService, Task<List<RoleData>>>(x => x.GetRolesAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<List<string>>(), default))
        .Returns(Task.FromResult(_rolesData));

      _mocker
        .Setup<ICompanyUserInfoMapper, CompanyUserInfo>(x => x.Map(It.IsAny<CompanyData>(), It.IsAny<CompanyUserData>()))
        .Returns(_companyUser);

      _mocker
        .Setup<IDepartmentInfoMapper, DepartmentUserInfo>(x => x.Map(It.IsAny<Guid>(), It.IsAny<DepartmentData>()))
        .Returns(_departmentUser);

      _mocker
        .Setup<IOfficeInfoMapper, OfficeInfo>(x => x.Map(It.IsAny<OfficeData>()))
        .Returns(_officeUser);

      _mocker
        .Setup<IPositionInfoMapper, PositionInfo>(x => x.Map(It.IsAny<PositionData>()))
        .Returns(_positionUser);

      _mocker
        .Setup<IRoleInfoMapper, RoleInfo>(x => x.Map(It.IsAny<RoleData>()))
        .Returns(_userRole);

      _mocker
        .Setup<IUserResponseMapper, UserResponse>(
          x => x.Map(
            It.IsAny<DbUser>(),
            It.IsAny<CompanyUserInfo>(),
            It.IsAny<ImageInfo>(),
            It.IsAny<DepartmentUserInfo>(),
            It.IsAny<List<ImageInfo>>(),
            It.IsAny<OfficeInfo>(),
            It.IsAny<PositionInfo>(),
            It.IsAny<RoleInfo>()))
        .Returns(_userResponse);

      _command = _mocker.CreateInstance<GetUserCommand>();
    }

    [Test]
    public void SuccessTest()
    {
      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_filter).Result);

      Verifiable(
        responseCreatorCalls: Times.Never(),
        userRepositoryCalls: Times.Once(),
        companyServiceCalls: Times.Once(),
        departmentServiceCalls: Times.Once(),
        imageServiceCalls: Times.Once(),
        officeServiceCalls: Times.Once(),
        positionServiceCalls: Times.Once(),
        rightServiceCalls: Times.Once(),
        companyUserInfoMapperCalls: Times.Once(),
        departmentInfoMapperCalls: Times.Once(),
        officeInfoMapperCalls: Times.Once(),
        positionInfoMapperCalls: Times.Once(),
        roleInfoMapperCalls: Times.Once(),
        userResponseMapperCalls: Times.Once());
    }

    [Test]
    public void SuccessWithoutCompanyServiceTest()
    {
      _filter.IncludeCompany = false;

      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_filter).Result);

      Verifiable(
        responseCreatorCalls: Times.Never(),
        userRepositoryCalls: Times.Once(),
        companyServiceCalls: Times.Never(),
        departmentServiceCalls: Times.Once(),
        imageServiceCalls: Times.Once(),
        officeServiceCalls: Times.Once(),
        positionServiceCalls: Times.Once(),
        rightServiceCalls: Times.Once(),
        companyUserInfoMapperCalls: Times.Once(),
        departmentInfoMapperCalls: Times.Once(),
        officeInfoMapperCalls: Times.Once(),
        positionInfoMapperCalls: Times.Once(),
        roleInfoMapperCalls: Times.Once(),
        userResponseMapperCalls: Times.Once());
    }

    [Test]
    public void SuccessWithoutDepartmentServiceTest()
    {
      _filter.IncludeDepartment = false;

      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_filter).Result);

      Verifiable(
        responseCreatorCalls: Times.Never(),
        userRepositoryCalls: Times.Once(),
        companyServiceCalls: Times.Once(),
        departmentServiceCalls: Times.Never(),
        imageServiceCalls: Times.Once(),
        officeServiceCalls: Times.Once(),
        positionServiceCalls: Times.Once(),
        rightServiceCalls: Times.Once(),
        companyUserInfoMapperCalls: Times.Once(),
        departmentInfoMapperCalls: Times.Once(),
        officeInfoMapperCalls: Times.Once(),
        positionInfoMapperCalls: Times.Once(),
        roleInfoMapperCalls: Times.Once(),
        userResponseMapperCalls: Times.Once());
    }

    [Test]
    public void SuccessWithoutImageServiceTest()
    {
      _filter.IncludeAvatars = false;
      _filter.IncludeCurrentAvatar = false;

      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_filter).Result);

      Verifiable(
        responseCreatorCalls: Times.Never(),
        userRepositoryCalls: Times.Once(),
        companyServiceCalls: Times.Once(),
        departmentServiceCalls: Times.Once(),
        imageServiceCalls: Times.Never(),
        officeServiceCalls: Times.Once(),
        positionServiceCalls: Times.Once(),
        rightServiceCalls: Times.Once(),
        companyUserInfoMapperCalls: Times.Once(),
        departmentInfoMapperCalls: Times.Once(),
        officeInfoMapperCalls: Times.Once(),
        positionInfoMapperCalls: Times.Once(),
        roleInfoMapperCalls: Times.Once(),
        userResponseMapperCalls: Times.Once());
    }

    [Test]
    public void SuccessWithoutOfficeServiceTest()
    {
      _filter.IncludeOffice = false;

      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_filter).Result);

      Verifiable(
        responseCreatorCalls: Times.Never(),
        userRepositoryCalls: Times.Once(),
        companyServiceCalls: Times.Once(),
        departmentServiceCalls: Times.Once(),
        imageServiceCalls: Times.Once(),
        officeServiceCalls: Times.Never(),
        positionServiceCalls: Times.Once(),
        rightServiceCalls: Times.Once(),
        companyUserInfoMapperCalls: Times.Once(),
        departmentInfoMapperCalls: Times.Once(),
        officeInfoMapperCalls: Times.Once(),
        positionInfoMapperCalls: Times.Once(),
        roleInfoMapperCalls: Times.Once(),
        userResponseMapperCalls: Times.Once());
    }

    [Test]
    public void SuccessWithoutPositionServiceTest()
    {
      _filter.IncludePosition = false;

      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_filter).Result);

      Verifiable(
        responseCreatorCalls: Times.Never(),
        userRepositoryCalls: Times.Once(),
        companyServiceCalls: Times.Once(),
        departmentServiceCalls: Times.Once(),
        imageServiceCalls: Times.Once(),
        officeServiceCalls: Times.Once(),
        positionServiceCalls: Times.Never(),
        rightServiceCalls: Times.Once(),
        companyUserInfoMapperCalls: Times.Once(),
        departmentInfoMapperCalls: Times.Once(),
        officeInfoMapperCalls: Times.Once(),
        positionInfoMapperCalls: Times.Once(),
        roleInfoMapperCalls: Times.Once(),
        userResponseMapperCalls: Times.Once());
    }

    [Test]
    public void SuccessWithoutRoleServiceTest()
    {
      _filter.IncludeRole = false;

      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_filter).Result);

      Verifiable(
        responseCreatorCalls: Times.Never(),
        userRepositoryCalls: Times.Once(),
        companyServiceCalls: Times.Once(),
        departmentServiceCalls: Times.Once(),
        imageServiceCalls: Times.Once(),
        officeServiceCalls: Times.Once(),
        positionServiceCalls: Times.Once(),
        rightServiceCalls: Times.Never(),
        companyUserInfoMapperCalls: Times.Once(),
        departmentInfoMapperCalls: Times.Once(),
        officeInfoMapperCalls: Times.Once(),
        positionInfoMapperCalls: Times.Once(),
        roleInfoMapperCalls: Times.Once(),
        userResponseMapperCalls: Times.Once());
    }

    [Test]
    public void BadRequestIfFilterIsNullTest()
    {
      _filter = null;

      SerializerAssert.AreEqual(_failureResponse, _command.ExecuteAsync(_filter).Result);

      Verifiable(
        responseCreatorCalls: Times.Once(),
        userRepositoryCalls: Times.Never(),
        companyServiceCalls: Times.Never(),
        departmentServiceCalls: Times.Never(),
        imageServiceCalls: Times.Never(),
        officeServiceCalls: Times.Never(),
        positionServiceCalls: Times.Never(),
        rightServiceCalls: Times.Never(),
        companyUserInfoMapperCalls: Times.Never(),
        departmentInfoMapperCalls: Times.Never(),
        officeInfoMapperCalls: Times.Never(),
        positionInfoMapperCalls: Times.Never(),
        roleInfoMapperCalls: Times.Never(),
        userResponseMapperCalls: Times.Never());
    }

    [Test]
    public void BadRequestIfUserIdAndEmailWereNotSpecifiedTest()
    {
      _filter.UserId = null;
      _filter.Email = null;

      SerializerAssert.AreEqual(_failureResponse, _command.ExecuteAsync(_filter).Result);

      Verifiable(
        responseCreatorCalls: Times.Once(),
        userRepositoryCalls: Times.Never(),
        companyServiceCalls: Times.Never(),
        departmentServiceCalls: Times.Never(),
        imageServiceCalls: Times.Never(),
        officeServiceCalls: Times.Never(),
        positionServiceCalls: Times.Never(),
        rightServiceCalls: Times.Never(),
        companyUserInfoMapperCalls: Times.Never(),
        departmentInfoMapperCalls: Times.Never(),
        officeInfoMapperCalls: Times.Never(),
        positionInfoMapperCalls: Times.Never(),
        roleInfoMapperCalls: Times.Never(),
        userResponseMapperCalls: Times.Never());
    }

    [Test]
    public void NotFoundResponseTest()
    {
      _mocker
        .Setup<IUserRepository, Task<DbUser>>(x => x.GetAsync(It.IsAny<GetUserFilter>()))
        .Returns(Task.FromResult((DbUser)null));

      SerializerAssert.AreEqual(_failureResponse, _command.ExecuteAsync(_filter).Result);

      Verifiable(
        responseCreatorCalls: Times.Once(),
        userRepositoryCalls: Times.Once(),
        companyServiceCalls: Times.Never(),
        departmentServiceCalls: Times.Never(),
        imageServiceCalls: Times.Never(),
        officeServiceCalls: Times.Never(),
        positionServiceCalls: Times.Never(),
        rightServiceCalls: Times.Never(),
        companyUserInfoMapperCalls: Times.Never(),
        departmentInfoMapperCalls: Times.Never(),
        officeInfoMapperCalls: Times.Never(),
        positionInfoMapperCalls: Times.Never(),
        roleInfoMapperCalls: Times.Never(),
        userResponseMapperCalls: Times.Never());
    }
  }
}
