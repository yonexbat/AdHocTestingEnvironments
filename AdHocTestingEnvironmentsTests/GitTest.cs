using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AdHocTestingEnvironmentsTests
{
    public class GitTest
    {
        [Fact]
        public async Task TestGit()
        {
            IConfiguration configuration = GetConfiguration();
            string localPath = configuration.GetValue<string>("path");
            string gitUrl = configuration.GetValue<string>("GitUrl");
            string gitUser = configuration.GetValue<string>("GitUser");
            string gitPassword = configuration.GetValue<string>("GitPw");


            string path = $"{localPath}{Path.DirectorySeparatorChar}gittest";

            Repository.Clone(gitUrl, path);

            //Create a file
            Guid guid = Guid.NewGuid();
            string guidString = guid.ToString();
            string newFileFileName = $"{guid}.txt";
            string fileNameBase = "base.txt";

            await File.WriteAllTextAsync($"{path}{Path.DirectorySeparatorChar}{newFileFileName}", guidString);
            await File.AppendAllTextAsync($"{path}{Path.DirectorySeparatorChar}{fileNameBase}", $"\n{guidString}");

            //Update a file


            using (Repository repo = new Repository(path))
            {
                StageChanges(repo, newFileFileName);
                StageChanges(repo, fileNameBase);
                CommitChanges(repo);
                PushChanges(repo, gitUser, gitPassword);
            }

            ForceDeleteDirectory(path);
        }

        private void StageChanges(Repository repo, string path)
        {
            Commands.Stage(repo, path);
        }

        private void CommitChanges(Repository repo)
        {
            Signature signature = new Signature("larrylaffer", "larrylaffer@sierra.com", DateTimeOffset.Now);
            {

            };

            repo.Commit("egotistic commit", signature, signature);
        }

        private void PushChanges(Repository repo, string userName, string pw)
        {
            var remote = repo.Network.Remotes["origin"];
            var options = new PushOptions()
            {
                CredentialsProvider = (_user, _valid, _hostname) =>
                {
                    return new UsernamePasswordCredentials { Username = userName, Password = pw, };
                },
            };

            Signature signature = new Signature("larrylaffer", "larrylaffer@sierra.com", DateTimeOffset.Now);
            {

            };

            // options.Credentials = credentials;
            var pushRefSpec = @"refs/heads/master";

            repo.Network.Push(remote, pushRefSpec, options);
        }

        private IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<KubernetesClientServiceTest>();

            return builder.Build();
        }

        private static void ForceDeleteDirectory(string path)
        {
            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };

            foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

            directory.Delete(true);
        }
    }
}
