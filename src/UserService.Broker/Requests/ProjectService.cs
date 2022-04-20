﻿using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Project;
using LT.DigitalOffice.Models.Broker.Requests.Project;
using LT.DigitalOffice.Models.Broker.Responses.Project;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests
{
  public class ProjectService : IProjectService
  {
    private readonly IRequestClient<IGetProjectsRequest> _rcGetProjects;
    private readonly ILogger<ProjectService> _logger;
    private readonly IGlobalCacheRepository _globalCache;

    public ProjectService(
      IRequestClient<IGetProjectsRequest> rcGetProjects,
      ILogger<ProjectService> logger,
      IGlobalCacheRepository globalCache)
    {
      _rcGetProjects = rcGetProjects;
      _logger = logger;
      _globalCache = globalCache;
    }

    public async Task<List<ProjectData>> GetProjectsAsync(Guid userId, List<string> errors)
    {
      (List<ProjectData> projects, int _) = await _globalCache
        .GetAsync<(List<ProjectData>, int)>(Cache.Projects, userId.GetRedisCacheHashCode());

      if (projects is not null)
      {
        _logger.LogInformation(
          "Project for user id '{UserId}' were taken from cache.",
          userId);
      }
      else
      {
        projects = (await RequestHandler.ProcessRequest<IGetProjectsRequest, IGetProjectsResponse>(
            _rcGetProjects,
            IGetProjectsRequest.CreateObj(userId: userId, includeUsers: true),
            errors,
            _logger))
          ?.Projects;
      }

      return projects;
    }
  }
}