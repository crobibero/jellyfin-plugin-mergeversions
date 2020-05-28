﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Collections;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Net;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Dto;
using MediaBrowser.Model.Tasks;
using Microsoft.Extensions.Logging;
using MediaBrowser.Api;

namespace Jellyfin.Plugin.TMDbBoxSets.ScheduledTasks
{
    public class RefreshLibraryTask : IScheduledTask
    {
        private readonly ILogger _logger;
        private readonly MergeVersionsManager _tmDbBoxSetManager;

        public RefreshLibraryTask(ILibraryManager libraryManager, ICollectionManager collectionManager, ILogger<VideosService> logger, IServerConfigurationManager serverConfigurationManager,
            IHttpResultFactory httpResultFactory,
            IUserManager userManager,
            IDtoService dtoService,
            IAuthorizationContext authContext)
        {
            _logger = logger;
            _tmDbBoxSetManager = new MergeVersionsManager(libraryManager, collectionManager, logger, serverConfigurationManager,
             httpResultFactory,
             userManager,
             dtoService,
             authContext, new Api.GetId());
        }
        public Task Execute(CancellationToken cancellationToken, IProgress<double> progress)
        {
            _logger.LogInformation("Starting plugin refresh library task");
            _tmDbBoxSetManager.ScanLibrary(progress);
            _logger.LogInformation("plugin refresh library task finished");
            return Task.CompletedTask;
        }

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            // Run this task every 24 hours
            return new[] {
                new TaskTriggerInfo { Type = TaskTriggerInfo.TriggerInterval, IntervalTicks = TimeSpan.FromHours(24).Ticks}
            };
        }

        public string Name => "Scan library to merge repeated movies";
        public string Key => "MergeVersionsRefreshLibraryTask";
        public string Description => "Scans all libraries for movies merge repeated movies";
        public string Category => "Merge Versions";
    }
}
