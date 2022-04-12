using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.TextHandlers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Responses.TextTemplate;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Communication;
using LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.UnitTests.Communication
{
  class CreateCommunicationCommandTests
  {
    private AutoMocker _mocker;
    private ICreateCommunicationCommand _command;

    private CreateCommunicationRequest _request = new();
    private DbUserCommunication _dbModel = new();
    private OperationResultResponse<Guid?> _response = new();
    private Guid? _communicationId = Guid.NewGuid();
    private Guid _requestUserId = Guid.NewGuid();
    private IGetTextTemplateResponse _textTemplateResponse;
    private IDictionary<object, object> _httpItems = new Dictionary<object, object>();

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new AutoMocker();
      _httpItems.Add("UserId", _requestUserId);

      _request.Type = Models.Dto.Enums.CommunicationType.Email;
      _request.UserId = _requestUserId;
      _request.Value = "phone";

      _response.Status = Kernel.Enums.OperationResultStatusType.FullSuccess;
      _response.Errors = new();
      _response.Body = _communicationId;
    }

    [SetUp]
    public void SetUp()
    {
      _mocker
        .Setup<ICreateCommunicationRequestValidator, Task<ValidationResult>>(x => x.ValidateAsync(It.IsAny<CreateCommunicationRequest>(), default))
        .Returns(Task.FromResult(new ValidationResult() { }));

      _mocker
        .Setup<IDbUserCommunicationMapper, DbUserCommunication>(x => x.Map(It.IsAny<CreateCommunicationRequest>()))
        .Returns(_dbModel);

      _mocker
        .Setup<IUserCommunicationRepository, Task<Guid?>>(x => x.CreateAsync(It.IsAny<DbUserCommunication>()))
        .Returns(Task.FromResult(_communicationId));

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(It.IsAny<int[]>()))
        .Returns(Task.FromResult(true));

      _mocker
        .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
        .Returns(_httpItems);

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<Guid?>>(x => x.CreateFailureResponse<Guid?>(It.IsAny<HttpStatusCode>(), default))
        .Returns(new OperationResultResponse<Guid?>());

      _mocker
        .Setup<IHttpContextAccessor, int>(x => x.HttpContext.Response.StatusCode)
        .Returns(0);

      /*var value = "";

      _mocker
        .Setup<IMemoryCache, bool>(x => x.TryGetValue(It.IsAny<object>(), out value))
        .Returns(true);*/

      /*_mocker
        .Setup<IOptions<MemoryCacheConfig>, double>(x => x.Value.CacheLiveInMinutes)
        .Returns(double.MinValue);*/

      _mocker
        .Setup<ITextTemplateParser, string>(x => x.Parse(It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()))
        .Returns(String.Empty);

      _mocker
        .Setup<ITextTemplateService, Task<IGetTextTemplateResponse>>(x =>
          x.GetAsync(It.IsAny<TemplateType>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Guid?>()))
        .Returns(Task.FromResult(_textTemplateResponse));

      _mocker
        .Setup<IEmailService, Task>(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
        .Returns(Task.CompletedTask);

      _command = _mocker.CreateInstance<CreateCommunicationCommand>();
    }

    [Test]
    public void SuccessTest()
    {
      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_request).Result);

      /*_mocker.Verify<IUserRepository, DbPendingUser>(
          x => x.GetPendingUser(It.IsAny<Guid>()),
          Times.Never());

      _mocker.Verify<IDbUserCredentialsMapper, DbUserCredentials>(
          x => x.Map(It.IsAny<CreateCredentialsRequest>(),
              It.IsAny<string>(),
              It.IsAny<string>()),
          Times.Never());

      _mocker.Verify<IRequestClient<IGetTokenRequest>, IOperationResult<IGetTokenResponse>>(
          x => x.GetResponse<IOperationResult<IGetTokenResponse>>(
              IGetTokenRequest.CreateObj(_userId),
              default,
              default)
          .Result.Message,
          Times.Never());

      _mocker.Verify<IUserCredentialsRepository, Guid>(
          x => x.Create(It.IsAny<DbUserCredentials>()),
          Times.Never());

      _mocker.Verify<IUserRepository>(
          x => x.DeletePendingUser(It.IsAny<Guid>()),
          Times.Never());

      _mocker.Verify<IUserRepository, bool>(
          x => x.SwitchActiveStatus(It.IsAny<Guid>(), It.IsAny<bool>()),
          Times.Never());*/
    }
  }
}
