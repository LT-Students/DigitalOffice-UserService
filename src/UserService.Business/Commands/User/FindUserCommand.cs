using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Filtres;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  public class FindUserCommand : IFindUserCommand
  {
    private readonly IBaseFindFilterValidator _baseFindValidator;
    private readonly IUserRepository _userRepository;
    private readonly IUserInfoMapper _userInfoMapper;
    private readonly IImageService _imageService;
    private readonly IResponseCreator _responseCreator;

    public FindUserCommand(
      IBaseFindFilterValidator baseFindValidator,
      IUserRepository userRepository,
      IUserInfoMapper userInfoMapper,
      IImageService imageService,
      IResponseCreator responseCreator)
    {
      _baseFindValidator = baseFindValidator;
      _userRepository = userRepository;
      _userInfoMapper = userInfoMapper;
      _imageService = imageService;
      _responseCreator = responseCreator;
    }

    public async Task<FindResultResponse<UserInfo>> ExecuteAsync(FindUsersFilter filter)
    {
      if (!_baseFindValidator.ValidateCustom(filter, out List<string> errors))
      {
        return _responseCreator.CreateFailureFindResponse<UserInfo>(HttpStatusCode.BadRequest, errors);
      }

      FindResultResponse<UserInfo> response = new();

      (List<DbUser> dbUsers, int totalCount) = await _userRepository.FindAsync(filter);

      List<ImageInfo> images = filter.IncludeCurrentAvatar
        ? await _imageService.GetImagesAsync(
          dbUsers
            .Where(u => u.Avatars.Any()).Select(u => u.Avatars.FirstOrDefault())
            .Select(ua => ua.AvatarId)
            .ToList(),
          response.Errors)
        : default;

      response.Body = new();
      response.Body
        .AddRange(dbUsers.Select(dbUser =>
        _userInfoMapper.Map(
          dbUser,
          images?.FirstOrDefault(i => i.Id == dbUser.Avatars.FirstOrDefault()?.AvatarId)
        )));

      response.TotalCount = totalCount;

      return response;
    }
  }
}
