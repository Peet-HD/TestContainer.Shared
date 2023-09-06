using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TestContainer.Shared.Models.Sonarqube;

namespace TestContainer.Shared.Containers
{
    public class SonarqubeContainer : IContainer
    {
        public string UserToken { get; private set; } = string.Empty;
        public string AnalysisToken { get; private set; } = string.Empty;

        #region IContainer Properties
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
        #endregion
        private readonly IContainer _container;
        public SonarqubeContainer(INetwork? network = null)
        {
            var containerbuilder = new ContainerBuilder()
                .WithAutoRemove(true)
                .WithImage("sonarqube:latest")
                .WithPortBinding(9000, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(9000))
                .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("SonarQube is operational"));

            if (network is not null) 
            {
                containerbuilder = containerbuilder.WithNetwork(network);
            }
            _container = containerbuilder.Build();
        }


        private static async Task<AnalyseTokenType?> GetToken(int port, string tokenName, string type)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:{port}/api/user_tokens/generate?name={tokenName}&type={type}");
            request.Headers.Add("Authorization", "Basic YWRtaW46YWRtaW4="); // base64: root:password
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AnalyseTokenType>(resultString, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
            });

            return result;
        }

        public async Task StartAsync(CancellationToken ct = default)
        {
            await _container.StartAsync(ct);

            AnalyseTokenType? analysisToken = await GetToken(_container.GetMappedPublicPort(9000), "analyseToken", "GLOBAL_ANALYSIS_TOKEN");
            AnalyseTokenType? userToken = await GetToken(_container.GetMappedPublicPort(9000), "userToken", "USER_TOKEN");

            if (analysisToken is not null) AnalysisToken = analysisToken.Token;
            if (userToken is not null) UserToken = userToken.Token;
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