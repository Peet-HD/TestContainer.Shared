using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using System.Text;
using TestContainer.Shared.Models;

namespace TestContainer.Shared.Containers;

public class SonarscannerCliContainer : BaseContainer
{
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
        container = containerBuilder.Build();
    }
}
