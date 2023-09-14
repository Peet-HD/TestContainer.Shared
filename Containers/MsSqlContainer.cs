using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestContainer.Shared.Containers
{
    internal class MsSqlContainer : IContainer
    {
        private readonly IContainer _container;
        public string Password { get; private set; }
        public string Username
        {
            get
            {
                return "sa";
            }
        }

        public int MqPort => _container.GetMappedPublicPort(1414);
        public int ManagementPort => _container.GetMappedPublicPort(9443);

        public MsSqlContainer(INetwork? network = null)
        {
            Password = Guid.NewGuid().ToString();
            var containerBuilder = new ContainerBuilder()
                .WithWaitStrategy(Wait.ForUnixContainer())
                .WithEnvironment(new Dictionary<string, string>
                {
                    { "ACCEPT_EULA", "Y"},
                    { "MSSQL_SA_PASSWORD", Password }
                })
                .WithAutoRemove(true)
                .WithPortBinding(1433, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest");

            if (network is not null)
            {
                containerBuilder = containerBuilder.WithNetwork(network);
            }
            _container = containerBuilder.Build();
        }

        public string GetConnectionString(string databaseName = "master")
        {
            var properties = new Dictionary<string, string>
            {
                { "Server", Hostname + "," + GetMappedPublicPort(1433) },
                { "Database",  databaseName},
                { "User Id", Username },
                { "Password", Password },
                { "TrustServerCertificate", bool.TrueString }
            };
            return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
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
