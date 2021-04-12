﻿using LT.DigitalOffice.UserService.Models.Broker.Models;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.Broker.Responses
{
    public interface IGetUsersDataResponse
    {
        List<UserData> UsersData { get; }

        static object CreateObj(List<UserData> usersData)
        {
            return new
            {
                UsersData = usersData
            };
        }
    }
}
