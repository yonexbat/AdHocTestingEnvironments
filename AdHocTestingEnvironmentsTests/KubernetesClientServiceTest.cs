﻿using AdHocTestingEnvironments.Services.Implementations;
using AdHocTestingEnvironments.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdHocTestingEnvironmentsTests
{
    public class KubernetesClientServiceTest
    {

        [Fact]
        public async Task StartOk()
        {

            //Arrange
            IKubernetesClientService client = CreateClient();

            //Act
            await client.StartEnvironment(new AdHocTestingEnvironments.Model.Kubernetes.CreateEnvironmentInstanceData()
            {
                Image = "claudeglauser/sample-webapp:latest",
                InitSqlScript = InitScript,
                Name = "testwzei",
            });
        }

        [Fact]
        public async Task StopOk()
        {
            //Arrange           
            IKubernetesClientService client = CreateClient();

            //Act
            await client.StopEnvironment("testwzei");
        }

        [Fact]
        public async Task GetEnvironmentsOk()
        {
            //Arrange           
            IKubernetesClientService client = CreateClient();

            var result = await client.GetEnvironments();
        }

        private IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<KubernetesClientServiceTest>();

            return builder.Build();

        }

        private IKubernetesClientService CreateClient()
        {
            IConfiguration configuration = GetConfiguration();
            string host = configuration.GetValue<string>("KubernetesHost");
            string token = configuration.GetValue<string>("KubernetesAccessToken");

            var inMemorySettings = new Dictionary<string, string> {
                {"KubernetesAccessToken", token},
                {"KubernetesHost", host},
                {"KubernetesNamespace", "default" }
            };

            IConfiguration mockConfiguration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var mockLogger = new Mock<ILogger<KubernetesClientService>>().Object;

            IKubernetesFactory factory = new KubernetesFactory(mockConfiguration, new Mock<ILogger<KubernetesFactory>>().Object);
            IKubernetesObjectBuilder objectBuilder = new KubernetesObjectBuilder();

            IKubernetesClientService client = new KubernetesClientService(mockConfiguration, factory, objectBuilder, mockLogger);
            return client;
        }

        private const string InitScript = @"
CREATE DATABASE test;
\connect test;

CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
    ""MigrationId"" character varying(150) NOT NULL,
    ""ProductVersion"" character varying(32) NOT NULL,
    CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY(""MigrationId"")
);

START TRANSACTION;

        CREATE TABLE ""GuestBookEntry"" (
    ""Id"" integer GENERATED BY DEFAULT AS IDENTITY,
    ""Text"" text NULL,
    CONSTRAINT ""PK_GuestBookEntry"" PRIMARY KEY(""Id"")
);

INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
VALUES('20210626104025_InitialCreate', '5.0.7');

COMMIT;
";
    }
}
