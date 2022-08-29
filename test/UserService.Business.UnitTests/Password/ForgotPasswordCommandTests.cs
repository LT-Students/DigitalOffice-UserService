using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Helpers.TextHandlers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Responses.TextTemplate;
using LT.DigitalOffice.UnitTestKernel;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Password;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.UnitTests.Password
{
  class ForgotPasswordCommandTests
  {
    private AutoMocker _mocker;
    private IForgotPasswordCommand _command;

    private string _emailRequest = "email@gmail.com";
    private string _loginRequest = "login";
    private DbUser _dbUser;
    private IDictionary<object, object> _items;
    private IMemoryCache _memoryCache;
    private IOptions<MemoryCacheConfig> _cacheOptions;
    private OperationResultResponse<string> _response;
    private OperationResultResponse<string> _failureResponse;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _cacheOptions = Options.Create(new MemoryCacheConfig
      {
        CacheLiveInMinutes = 5
      });

      var userId = Guid.NewGuid();

      _dbUser = new DbUser
      {
        Id = userId,
        Communications = new List<DbUserCommunication>
        {
          new DbUserCommunication
          {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = (int)CommunicationType.Email,
            Value = _emailRequest
          }
        }
      };

      _items = new Dictionary<object, object>();
      _items.Add("UserId", userId);

      _response = new OperationResultResponse<string>(body: _dbUser.Communications.First().Value);

    }

    [SetUp]
    public void SetUp()
    {
      /*_mocker
        .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
        .Returns(_items);*/

      _failureResponse = new OperationResultResponse<string>(errors: new List<string>() { "Error" });

      _mocker = new AutoMocker();

      _mocker
        .Setup<IResponseCreator, OperationResultResponse<string>>(x => x
          .CreateFailureResponse<string>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()))
        .Returns(_failureResponse);

      _mocker
        .Setup<IUserRepository, Task<DbUser>>(x => x.GetAsync(It.IsAny<GetUserFilter>()))
        .Returns(Task.FromResult(_dbUser));

      _mocker
        .Setup<IGeneratePasswordCommand, string>(x => x.Execute())
        .Returns(String.Empty);

      /*_mocker
        .Setup<ICacheEntry, ICacheEntry>(x => x.SetOptions(It.IsAny<MemoryCacheEntryOptions>()))
        .Returns();

      _mocker
        .Setup<IMemoryCache, ICacheEntry>(x => x.CreateEntry(It.IsAny<object>())))
        .Returns(Guid.Empty);*/

      _mocker
        .Setup<IGetTextTemplateResponse, string>(x => x.Text)
        .Returns(string.Empty);

      _mocker
        .Setup<IGetTextTemplateResponse, string>(x => x.Subject)
        .Returns(string.Empty);

      _mocker
        .Setup<ITextTemplateParser, string>(x => x.ParseModel(It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()))
        .Returns(_mocker.GetMock<IGetTextTemplateResponse>().Object.Text);

      _mocker
        .Setup<ITextTemplateParser, string>(x => x.Parse(It.IsAny<Dictionary<string, string>>(), It.IsAny<string>()))
        .Returns(_mocker.GetMock<IGetTextTemplateResponse>().Object.Text);

      _mocker
        .Setup<ITextTemplateService, Task<IGetTextTemplateResponse>>(x => x
          .GetAsync(It.IsAny<TemplateType>(), It.IsAny<string>(), It.IsAny<List<string>>(), It.IsAny<Guid?>()))
        .Returns(Task.FromResult(_mocker.GetMock<IGetTextTemplateResponse>().Object));

      _mocker
        .Setup<IEmailService, Task>(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<string>>()))
        .Returns(Task.CompletedTask);

      _command = _mocker.CreateInstance<ForgotPasswordCommand>();
    }

    /*[Test]
    public void SuccessTest()
    {
      SerializerAssert.AreEqual(_response, _command.ExecuteAsync(_emailRequest).Result);
    }*/
  }
}
