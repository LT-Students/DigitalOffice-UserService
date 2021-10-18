using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.Models.Broker.Responses.Search;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Consumers
{
  public class SearchUsersConsumer : IConsumer<ISearchUsersRequest>
  {
    private IUserRepository _userRepository;

    private async Task<object> SearchUsersAsync(string text)
    {
      List<DbUser> users = await _userRepository.SearchAsync(text);

      return ISearchResponse.CreateObj(
        users.Select(
          u => new SearchInfo(u.Id, string.Join(" ", u.LastName, u.FirstName))).ToList());
    }

    public SearchUsersConsumer(
      IUserRepository userRepository)
    {
      _userRepository = userRepository;
    }

    public async Task Consume(ConsumeContext<ISearchUsersRequest> context)
    {
      var response = OperationResultWrapper.CreateResponse(SearchUsersAsync, context.Message.Value);

      await context.RespondAsync<IOperationResult<ISearchResponse>>(response);
    }
  }
}
