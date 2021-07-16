using LT.DigitalOffice.Models.Broker.Requests.User;
using LT.DigitalOffice.UserService.Mappers.Db;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.UnitTests
{
    public class DbUserMapperTests
    {
        private IDbUserMapper _mapper;

        private Guid imageId = Guid.NewGuid();

        [SetUp]
        public void SetUp()
        {
            _mapper = new DbUserMapper();
        }

        [Test]
        public void MapNullCreateUserRequestTest()
        {
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null, null));
            Assert.Throws<ArgumentNullException>(() => _mapper.Map(null));
        }

        [Test]
        public void ShouldMapCreateAdminRequest()
        {
            const string login = "admin";
            const string password = "password";
            const string firstName = "spartak";
            const string lastName = "ryabtsev";
            const string email = "email";

            var request = new Mock<ICreateAdminRequest>();
            request
                .Setup(x => x.FirstName)
                .Returns(firstName);
            request
                .Setup(x => x.LastName)
                .Returns(lastName);
            request
                .Setup(x => x.Email)
                .Returns(email);
            request
                .Setup(x => x.Password)
                .Returns(password);
            request
                .Setup(x => x.Login)
                .Returns(login);

            DbUser result = _mapper.Map(request.Object);

            Assert.AreNotEqual(Guid.Empty, result.Id);
            Assert.IsTrue(result.IsAdmin);
            Assert.IsTrue(result.IsActive);
            Assert.AreEqual(firstName, result.FirstName);
            Assert.AreEqual(lastName, result.LastName);

            var communication = result.Communications.ToList();

            Assert.AreEqual(1, communication.Count);
            Assert.AreEqual((int)CommunicationType.Email, communication[0].Type);
            Assert.AreEqual(email, communication[0].Value);
            Assert.AreEqual(result.Id, communication[0].UserId);
            Assert.AreNotEqual(Guid.Empty, communication[0].Id);
        }

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