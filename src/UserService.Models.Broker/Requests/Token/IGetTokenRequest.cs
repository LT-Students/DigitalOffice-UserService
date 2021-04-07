﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using System;

namespace LT.DigitalOffice.Broker.Requests
{
    [AutoInjectRequest(nameof(RabbitMqConfig.GetTokenEndpoint))]
    public interface IGetTokenRequest
    {
        Guid UserId { get; }

        static object CreateObj(Guid userId)
        {
            return new
            {
                UserId = userId
            };
        }
    }
}