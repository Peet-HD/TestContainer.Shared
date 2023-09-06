using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using System.Text;

namespace TestContainer.Shared.Containers
{
    public class SonarscannerCliContainer
    {
        private IContainer? _container;
        private readonly string _uri;
        private readonly string _token;
        private readonly string _projectKey;
        private readonly List<(string fileName, string fileContent)> _files;
        private readonly INetwork? _network;

        public SonarscannerCliContainer(string uri, string token, string projectKey, List<(string fileName, string fileContent)> files, INetwork? network = null)
        {
            _uri = uri;
            _token = token;
            _projectKey = projectKey;
            _files = files;
            _network = network;
        }

        private void Setup()
        {
            var containerBuilder = new ContainerBuilder()
                .WithWaitStrategy(Wait.ForUnixContainer())
                .WithEnvironment(new Dictionary<string, string>
                {
                    { "SONAR_HOST_URL", _uri },
                    { "SONAR_TOKEN", _token},
                    { "SONAR_SCANNER_OPTS", $"-Dsonar.projectKey={_projectKey}"}
                })
                .WithImage("sonarsource/sonar-scanner-cli");

            foreach (var file in _files)
            {
                var contentArray = Encoding.ASCII.GetBytes(file.fileContent);
                containerBuilder = containerBuilder.WithResourceMapping(resourceContent: contentArray, $"/usr/src/{file.fileName}");
            }

            if (_network is not null)
            {
                containerBuilder = containerBuilder.WithNetwork(_network);
            }
            _container = containerBuilder.Build();
        }
        public async Task StartAsync()
        {
            Setup();
            await _container!.StartAsync();
        }

        public async Task StopAsync() => await _container!.StopAsync();

    }
}
