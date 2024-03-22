using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using Microsoft.Extensions.Logging;

namespace TestContainer.Shared.Models;

public class BaseContainer : IContainer
{
    internal IContainer container;
    public BaseContainer()
    {
        container = new ContainerBuilder().Build();

        container.Creating += Creating;
        container.Created += Created;
        container.Starting += Starting;
        container.Started += Started;
        container.Stopping += Stopping;
        container.Stopped += Stopped;
    }
    public async Task StartAsync() => await container.StartAsync();

    public async Task StopAsync() => await container.StopAsync();

    public ILogger Logger => container.Logger;

    public string Id => container.Id;

    public string Name => container.Name;

    public string IpAddress => container.IpAddress;

    public string MacAddress => container.MacAddress;

    public string Hostname => container.Hostname;

    public IImage Image => container.Image;

    public TestcontainersStates State => container.State;

    public TestcontainersHealthStatus Health => container.Health;

    public long HealthCheckFailingStreak => container.HealthCheckFailingStreak;

    public event EventHandler? Creating;
    public event EventHandler? Starting;
    public event EventHandler? Stopping;
    public event EventHandler? Created;
    public event EventHandler? Started;
    public event EventHandler? Stopped;

    public DateTime CreatedTime => container.CreatedTime;

    public DateTime StartedTime => container.StartedTime;

    public DateTime StoppedTime => container.StoppedTime;

    public async Task StartAsync(CancellationToken ct = default) => await container.StartAsync(ct);
    public async Task StopAsync(CancellationToken ct = default) => await container.StopAsync(ct);
    public ushort GetMappedPublicPort(int containerPort) => container.GetMappedPublicPort(containerPort);

    public ushort GetMappedPublicPort(string containerPort) => container.GetMappedPublicPort(containerPort);

    public Task<long> GetExitCodeAsync(CancellationToken ct = default) => container.GetExitCodeAsync(ct);

    public Task<(string Stdout, string Stderr)> GetLogsAsync(DateTime since = default, DateTime until = default, bool timestampsEnabled = true, CancellationToken ct = default) => container.GetLogsAsync(since, until, timestampsEnabled, ct);

    public Task CopyAsync(byte[] fileContent, string filePath, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead, CancellationToken ct = default) => container.CopyAsync(fileContent, filePath, fileMode, ct);
    public Task CopyAsync(string source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead, CancellationToken ct = default) => container.CopyAsync(source, target, fileMode, ct);
    public Task CopyAsync(DirectoryInfo source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead, CancellationToken ct = default) => container.CopyAsync(source, target, fileMode, ct);
    public Task CopyAsync(FileInfo source, string target, UnixFileModes fileMode = UnixFileModes.OtherRead | UnixFileModes.GroupRead | UnixFileModes.UserWrite | UnixFileModes.UserRead, CancellationToken ct = default) => container.CopyAsync(source, target, fileMode, ct);
    public Task<byte[]> ReadFileAsync(string filePath, CancellationToken ct = default) => container.ReadFileAsync(filePath, ct);
    public Task<ExecResult> ExecAsync(IList<string> command, CancellationToken ct = default) => container.ExecAsync(command, ct);
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return container.DisposeAsync();
    }
}
