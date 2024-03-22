using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using TestContainer.Shared.Models;

namespace TestContainer.Shared.Containers;

internal class MsSqlContainer : BaseContainer
{
    public string Password { get; private set; }
    public static string Username => "sa";

    public int SqlPort => container.GetMappedPublicPort(1414);
    public int ManagementPort => container.GetMappedPublicPort(9443);

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
        container = containerBuilder.Build();
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
}
