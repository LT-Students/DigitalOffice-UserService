using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.Pending.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.PendingUser.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Pending
{
  public class FindPendingUserCommand : IFindPendingUserCommand
  {
    private readonly IBaseFindFilterValidator _baseFindValidator;
    private readonly IPendingUserRepository _repository;
    private readonly IUserInfoMapper _mapper;
    private readonly IImageService _imageService;
    private readonly IResponseCreator _responseCreator;

    public FindPendingUserCommand(
      IBaseFindFilterValidator baseFindValidator,
      IPendingUserRepository repository,
      IUserInfoMapper mapper,
      IImageService imageService,
      IResponseCreator responseCreator)
    {
      _baseFindValidator = baseFindValidator;
      _repository = repository;
      _mapper = mapper;
      _imageService = imageService;
      _responseCreator = responseCreator;
    }

    public async Task<FindResultResponse<UserInfo>> ExecuteAsync(FindPendingUserFilter filter)
    {
      if (!_baseFindValidator.ValidateCustom(filter, out List<string> errors))
      {
        return _responseCreator.CreateFailureFindResponse<UserInfo>(HttpStatusCode.BadRequest, errors);
      }

      FindResultResponse<UserInfo> response = new();

      (List<DbPendingUser> dbPendingUsers, int totalCount) = await _repository.FindAsync(filter);

      List<ImageInfo> images = filter.IncludeCurrentAvatar
        ? await _imageService.GetImagesAsync(
          dbPendingUsers.SelectMany(x => x.User.Avatars.Where(v => v.IsCurrentAvatar)).Select(y => y.AvatarId).ToList(),
          response.Errors)
        : default;

      response.TotalCount = totalCount;
      response.Body = dbPendingUsers
        .Select(pu => _mapper
          .Map(
            pu.User,
            images?.FirstOrDefault(ii => ii.Id == pu.User.Avatars?.FirstOrDefault()?.AvatarId)))
        .ToList();

      return response;
    }
  }
}
