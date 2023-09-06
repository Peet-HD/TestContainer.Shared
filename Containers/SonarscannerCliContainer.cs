using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging;
using System.Text;

namespace TestContainer.Shared.Containers
{
    public class SonarscannerCliContainer : IContainer
    {
        private readonly IContainer _container;

        public SonarscannerCliContainer(string uri, string token, string projectKey, List<(string fileName, string fileContent)> files, INetwork? network = null)
        {
            var containerBuilder = new ContainerBuilder()
                .WithWaitStrategy(Wait.ForUnixContainer())
                .WithEnvironment(new Dictionary<string, string>
                {
                                { "SONAR_HOST_URL", uri },
                                { "SONAR_TOKEN", token},
                                { "SONAR_SCANNER_OPTS", $"-Dsonar.projectKey={projectKey}"}
                })
                .WithImage("sonarsource/sonar-scanner-cli");

            foreach (var (fileName, fileContent) in files)
            {
                var contentArray = Encoding.ASCII.GetBytes(fileContent);
                containerBuilder = containerBuilder.WithResourceMapping(resourceContent: contentArray, $"/usr/src/{fileName}");
            }

            if (network is not null)
            {
                containerBuilder = containerBuilder.WithNetwork(network);
            }
            _container = containerBuilder.Build();
        }

        public async Task StartAsync() => await _container.StartAsync();

        public async Task StopAsync() => await _container.StopAsync();

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

        public async Task StartAsync(CancellationToken ct = default) => await _container.StartAsync(ct);
        public async Task StopAsync(CancellationToken ct = default) => await _container.StopAsync(ct);
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
    }
}
