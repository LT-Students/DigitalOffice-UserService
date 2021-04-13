using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using System;

namespace LT.DigitalOffice.UserService.Mappers.UnitTests
{
    public class DbUserMapperTests
    {
        private IDbUserMapper _mapper;

        private Guid imageId = Guid.NewGuid();

        //[SetUp]
        //public void SetUp()
        //{
        //    _mapper = new DbUserMapper();
        //}

        //[Test]
        //public void MapNullCreateUserRequestTest()
        //{
        //    Assert.Throws<ArgumentNullException>(() => _mapper.Map(null, null));
        //}

        //[Test]
        //public void MapTest()
        //{
        //    const string login = "admin";
        //    const string password = "password";
        //    const string firstName = "spartak";
        //    const string lastName = "ryabtsev";
        //    const string email = "spartak.r@gmail.com";

        //    var request = new CreateUserRequest
        //    {
        //        Password = password,
        //        FirstName = firstName,
        //        LastName = lastName,
        //        Communications = new List<CommunicationInfo>
        //        {
        //            new CommunicationInfo
        //            {
        //                Type = CommunicationType.Email,
        //                Value = email
        //            }
        //        }
        //    };

        //    DbUser result = _mapper.Map(request, imageId);

        //    string passwordHash = UserPasswordHash.GetPasswordHash(login, result.Credentials.Salt, password);

        //    var expected = new DbUser
        //    {
        //        Id = result.Id,
        //        CreatedAt = result.CreatedAt,
        //        IsActive = true,
        //        FirstName = firstName,
        //        LastName = lastName,
        //        AvatarFileId = imageId,
        //        Credentials = new DbUserCredentials
        //        {
        //            Id = result.Credentials.Id,
        //            UserId = result.Id,
        //            Login = login,
        //            PasswordHash = passwordHash,
        //            Salt = result.Credentials.Salt
        //        },
        //        Communications = new HashSet<DbUserCommunication>
        //        {
        //            new DbUserCommunication
        //            {
        //                Id = result.Communications.FirstOrDefault().Id,
        //                Type = (int)CommunicationType.Email,
        //                Value = email,
        //                UserId = result.Id
        //            }
        //        }
        //    };

        //    SerializerAssert.AreEqual(expected, result);
        //}

        //[Test]
        //public void MapBadAvatarTest()
        //{
        //    const string login = "admin";
        //    const string password = "password";
        //    const string firstName = "spartak";
        //    const string lastName = "ryabtsev";

        //    var request = new CreateUserRequest
        //    {
        //        Password = password,
        //        FirstName = firstName,
        //        LastName = lastName
        //    };

        //    DbUser result = _mapper.Map(request, null);

        //    string passwordHash = UserPasswordHash.GetPasswordHash(login, result.Credentials.Salt, password);

        //    var expected = new DbUser
        //    {
        //        Id = result.Id,
        //        CreatedAt = result.CreatedAt,
        //        IsActive = true,
        //        FirstName = firstName,
        //        LastName = lastName,
        //        AvatarFileId = null,
        //        Credentials = new DbUserCredentials
        //        {
        //            Id = result.Credentials.Id,
        //            UserId = result.Id,
        //            Login = login,
        //            PasswordHash = passwordHash,
        //            Salt = result.Credentials.Salt
        //        },
        //        Communications = null
        //    };

        //    SerializerAssert.AreEqual(expected, result);
        //}

        //[Test]
        //public void MapExceptionAvatarTest()
        //{
        //    const string login = "admin";
        //    const string password = "password";
        //    const string firstName = "spartak";
        //    const string lastName = "ryabtsev";

        //    var request = new CreateUserRequest
        //    {
        //        Password = password,
        //        FirstName = firstName,
        //        LastName = lastName
        //    };

        //    DbUser result = _mapper.Map(request, null);

        //    string passwordHash = UserPasswordHash.GetPasswordHash(login, result.Credentials.Salt, password);

        //    var expected = new DbUser
        //    {
        //        Id = result.Id,
        //        CreatedAt = result.CreatedAt,
        //        IsActive = true,
        //        FirstName = firstName,
        //        LastName = lastName,
        //        AvatarFileId = null,
        //        Credentials = new DbUserCredentials
        //        {
        //            Id = result.Credentials.Id,
        //            UserId = result.Id,
        //            Login = login,
        //            PasswordHash = passwordHash,
        //            Salt = result.Credentials.Salt
        //        },
        //        Communications = null
        //    };

        //    SerializerAssert.AreEqual(expected, result);
        //}
    }
}
