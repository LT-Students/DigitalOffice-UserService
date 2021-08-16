using LT.DigitalOffice.UserService.Mappers.Db;
using NUnit.Framework;

namespace LT.DigitalOffice.UserService.Mappers.RequestsMappers.UnitTests
{
    /*internal class UserRequestMapperTests
    {
        private DbUserMapper userRequestMapper;

        [SetUp]
        public void SetUp()
        {
            userRequestMapper = new DbUserMapper();
        }

        #region EditUserRequest to DbUser
        //[Test]
        //public void ShouldReturnNewDbUserWhenDataCorrect()
        //{
        //    var request = new CreateUserRequest()
        //    {
        //        Login = "Example",
        //        FirstName = "Example",
        //        LastName = "Example",
        //        MiddleName = "Example",
        //        Status = UserStatus.Sick,
        //        Password = "Example",
        //        IsAdmin = false,
        //        Communications = new List<Communications>()
        //        {
        //            new Communications()
        //            {
        //                Type = CommunicationType.Email,
        //                Value = "Ex@mail.ru"
        //            }
        //        }
        //    };

        //    var result = userRequestMapper.Map(request, null);
        //    var connectionId = result.Communications.FirstOrDefault().Id;
        //    Guid userId = Guid.NewGuid();
        //    var user = new DbUser()
        //    {
        //        Id = userId,
        //        FirstName = "Example",
        //        LastName = "Example",
        //        MiddleName = "Example",
        //        Status = 1,
        //        IsAdmin = false,
        //        IsActive = true,
        //        Communications = new List<DbUserCommunication>
        //        {
        //            new DbUserCommunication()
        //            {
        //                Id = connectionId,
        //                Type = (int)CommunicationType.Email,
        //                Value = "Ex@mail.ru",
        //                UserId = userId
        //            }
        //        }
        //    };
        //    SerializerAssert.AreEqual(user, result);
        //}

        //[Test]
        //public void ShouldReturnNewDbUserWhenDataWithEmptyConnections()
        //{
        //    var request = new CreateUserRequest()
        //    {
        //        Login = "Example",
        //        FirstName = "Example",
        //        LastName = "Example",
        //        MiddleName = "Example",
        //        Status = UserStatus.Sick,
        //        Password = "Example",
        //        IsAdmin = false,
        //        Communications = null
        //    };

        //    var user = new DbUser()
        //    {
        //        Id = Guid.NewGuid(),
        //        FirstName = "Example",
        //        LastName = "Example",
        //        MiddleName = "Example",
        //        Status = 1,
        //        IsAdmin = false,
        //        IsActive = true,
        //        AvatarFileId = null,
        //        Communications = null
        //    };

        //    var dbUser = userRequestMapper.Map(request, null);
        //    user.Id = dbUser.Id;

        //    SerializerAssert.AreEqual(user, dbUser);
        //}

        //[Test]
        //public void ShouldThrowExceptionWhenRequestIsNull()
        //{
        //    CreateUserRequest request = null;

        //    Assert.Throws<BadRequestException>(() => userRequestMapper.Map(request, null, null));
        //}
        #endregion
    }*/
}