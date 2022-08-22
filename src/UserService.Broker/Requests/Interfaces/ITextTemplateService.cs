﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Responses.TextTemplate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface ITextTemplateService
  {
    Task<IGetTextTemplateResponse> GetAsync(
      TemplateType templateType,
      string locale,
      List<string> errors,
      Guid? endpointId = null);
  }
}
