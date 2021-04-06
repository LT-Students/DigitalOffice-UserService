using LT.DigitalOffice.UserService.Business.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Responses.User;

namespace LT.DigitalOffice.UserService.Business
{
    public class FindUserCommand : IFindUserCommand
    {
        /// <inheritdoc/>
        private readonly IUserRepository _repository;
        private readonly IUserInfoMapper _mapper;

        public FindUserCommand(
            IUserRepository repository,
            IUserInfoMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public UsersResponse Execute(int skipCount, int takeCount)
        {
            var dbUsers = _repository.Find(skipCount, takeCount, out int totalCount);

            UsersResponse result = new()
            {
                TotalCount = totalCount,
            };

            foreach (DbUser dbuser in dbUsers)
            {
                result.Users.Add(_mapper.Map(dbuser));
            }

            return result;
        }
    }
}
