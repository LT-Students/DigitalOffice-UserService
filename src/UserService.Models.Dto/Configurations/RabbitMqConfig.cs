﻿using LT.DigitalOffice.Kernel.Configurations;

namespace LT.DigitalOffice.UserService.Models.Dto.Configurations
{
    public class RabbitMqConfig : BaseRabbitMqConfig
    {
        public string CompanyServiceUrl { get; set; }
        public string GetUserCredentialsEndpoint { get; set; }
        public string GetUserDataEndpoint { get; set; }
        public string GetUsersDataEndpoint { get; set; }
        public string AddImageEndpoint { get; set; }
        public string GetFileEndpoint { get; set; }
        public string GetDepartmentEndpoint { get; set; }
        public string GetPositionEndpoint { get; set; }
        public string ChangeUserDepartmentEndpoint { get; set; }
        public string ChangeUserPositionEndpoint { get; set; }
        public string GetProjectIdsEndpoint { get; set; }
        public string GetProjectInfoEndpoint { get; set; }
        public string GetUserProjectsInfoEndpoint { get; set; }
        public string SendEmailEndpoint { get; set; }
        public string GetTempalateTagsEndpoint { get; set; }
        public string GetTokenEndpoint { get; set; }
    }
}
