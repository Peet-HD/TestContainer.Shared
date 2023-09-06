using System.Net;
using System.Text.RegularExpressions;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Microsoft.Extensions.Logging;

namespace TestContainer.Shared.Containers
{
    public class GitlabContainer : IContainer
    {
        public string RootPassword { get; private set; } = string.Empty;

        public ILogger Logger => _container.Logger;

        public string Id => _container.Id;

        public string Name => _container.Name;

        public string IpAddress => _container.IpAddress;

        public string MacAddress => _container.MacAddress;

        public string Hostname => _container.Hostname;

        public IImage Image => _container.Image;

        public TestcontainersStates State => _container.State;

        public TestcontainersHealthStatus Health => _container.Health;

        public long HealthCheckFailingStreak => _container.HealthCheckFailingStreak;

        public event EventHandler? Creating;
        public event EventHandler? Starting;
        public event EventHandler? Stopping;
        public event EventHandler? Created;
        public event EventHandler? Started;
        public event EventHandler? Stopped;

        private readonly IContainer _container;

        public GitlabContainer()
        {
            var containerName = "TestContainer_Gitlab_" + Guid.NewGuid().ToString();
            _container = new ContainerBuilder()
                .WithAutoRemove(true)
                .WithCleanUp(true)
                .WithName(containerName)
                .WithImage("gitlab/gitlab-ce")
                .WithPortBinding(80, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilFileExists("/etc/gitlab/initial_root_password"))
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(80))
                .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
                .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request.ForPath("/users/sign_in").ForStatusCode(HttpStatusCode.OK)))
                .Build();


            _container.Creating += Creating;
            _container.Starting += Starting;
            _container.Stopping += Stopping;
            _container.Created += Created;
            _container.Started += Started;
            _container.Stopped += Stopped;
        }


        private async Task<string> RetrieveRootPassword()
        {
            var byteArray = await _container.ReadFileAsync("/etc/gitlab/initial_root_password");

            string fileContent = System.Text.Encoding.UTF8.GetString(byteArray);
            string pattern = @"Password: .*";

            RegexOptions options = RegexOptions.IgnoreCase;
            var match = Regex.Match(fileContent, pattern, options);
            var splits = match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            RootPassword = splits[1];
            return splits[1];
        }

        public async Task StartAsync(CancellationToken ct = default)
        {
            await _container.StartAsync(ct);
            _ = await RetrieveRootPassword();
        }

        public async Task StopAsync(CancellationToken ct = default)
        {
            await _container.StopAsync(ct);
        }

        #region IContainer Implementation
        public ushort GetMappedPublicPort(int containerPort) => _container.GetMappedPublicPort(containerPort);

        public ushort GetMappedPublicPort(string containerPort) => _container.GetMappedPublicPort(containerPort);

        public Task<long> GetExitCodeAsync(CancellationToken ct = default) => _container.GetExitCodeAsync(ct);

        public Task<(string Stdout, string Stderr)> GetLogsAsync(DateTime since = default, DateTime until = default, bool timestampsEnabled = true, CancellationToken ct = default) => _container.GetLogsAsync(since, until, timestampsEnabled, ct);


        public Task CopyAsync(byte[] fileContent, string filePath, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead, CancellationToken ct = default) => _container.CopyAsync(fileContent, filePath, fileMode, ct);
        public Task CopyAsync(string source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead, CancellationToken ct = default) => _container.CopyAsync(source, target, fileMode, ct);
        public Task CopyAsync(DirectoryInfo source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead, CancellationToken ct = default) => _container.CopyAsync(source, target, fileMode, ct);
        public Task CopyAsync(FileInfo source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead, CancellationToken ct = default) => _container.CopyAsync(source, target, fileMode, ct);
        public Task<byte[]> ReadFileAsync(string filePath, CancellationToken ct = default) => _container.ReadFileAsync(filePath, ct);
        public Task<ExecResult> ExecAsync(IList<string> command, CancellationToken ct = default) => _container.ExecAsync(command, ct);
        public ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return _container.DisposeAsync();
        }
        #endregion
    }
}