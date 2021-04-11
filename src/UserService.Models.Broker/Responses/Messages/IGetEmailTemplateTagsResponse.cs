using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.Broker.Responses
{
    [AutoInjectRequest(nameof(RabbitMqConfig.GetTempalateTagsEndpoint))]
    public interface IGetEmailTemplateTagsResponse
    {
        Guid TemplateId { get; set; }
        IDictionary<string, string> TemplateTags { get; }

        static object CreateObj(Guid templateId, IDictionary<string, string> templateTags)
        {
            return new
            {
                TemplateId = templateId,
                TemplateTags = templateTags
            };
        }

        public IDictionary<string, string> CreateDictionaryTemplate(
            string userFirstName,
            string userEmail,
            string userId,
            string userPassword,
            string secret)
        {
            Func<string, bool> isKey = arg => !string.IsNullOrEmpty(arg) && TemplateTags.ContainsKey(nameof(arg));

            if (isKey(userFirstName))
            {
                TemplateTags[nameof(userFirstName)] = userFirstName;
            }

            if (isKey(userEmail))
            {
                TemplateTags[nameof(userEmail)] = userEmail;
            }

            if (isKey(userId))
            {
                TemplateTags[nameof(userId)] = userId;
            }

            if (isKey(secret))
            {
                TemplateTags[nameof(secret)] = secret;
            }

            return TemplateTags;
        }
    }
}
