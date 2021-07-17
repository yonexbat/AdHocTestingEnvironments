﻿using AdHocTestingEnvironments.Services.Interfaces;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AdHocTestingEnvironments.Services.Implementations
{
    public class GitClientService : IGitClientService
    {

        private readonly string _gitUrl;
        private readonly ILogger _logger;

        public GitClientService(IConfiguration configuration, ILogger<GitClientService> logger)
        {
            _gitUrl = configuration.GetValue<string>("GitUrl");
            _logger = logger;
        }

        public async Task CheckOut()
        {
            string localPath = $"{Path.GetTempPath()}git";
            _logger.LogInformation("Local git path: {0}, Git Repo: {1}", localPath, _gitUrl);
            
            try
            {
                // delete directory first.
                ForceDeleteDirectory(localPath);
                Repository.Clone(_gitUrl, localPath);
            } 
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private void ForceDeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                _logger.LogInformation("Directory already exist. Will delete it and recreate it");
                var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

                foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
                {
                    info.Attributes = FileAttributes.Normal;
                }

                directory.Delete(true);
            } 
            else
            {
                _logger.LogInformation("Directory does not exist");
            }
        }
    }
}
