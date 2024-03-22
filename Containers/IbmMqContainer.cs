using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using TestContainer.Shared.Models;

namespace TestContainer.Shared.Containers;

public class IbmMqContainer : BaseContainer
{
    public string AppUser { get; private set; }
    public string AppPassword { get; private set; }
    public string AdminUser { get; private set; }
    public string AdminPassword { get; private set; }
    public string QueueManager { get; private set; }
    public string Channel { get; private set; }
    public int MqPort => container.GetMappedPublicPort(1414);
    public int ManagementPort => container.GetMappedPublicPort(9443);

    public IbmMqContainer(bool devMode = true, INetwork? network = null)
    {

        AppPassword = Guid.NewGuid().ToString();
        QueueManager = Guid.NewGuid().ToString();
        AdminPassword = Guid.NewGuid().ToString();
        Channel = "SYSTEM.DEF.SVRCONN";
        AdminUser = "admin";
        AppUser = "app";
        var containerBuilder = new ContainerBuilder()
            .WithWaitStrategy(Wait.ForUnixContainer())
            .WithEnvironment(new Dictionary<string, string>
            {
                { "MQ_APP_PASSWORD",  AppPassword},
                { "MQ_ADMIN_PASSWORD", AdminPassword },
                { "LICENSE", "accept"},
                { "MQ_DEV", devMode.ToString() }
            })
            .WithAutoRemove(true)
            .WithPortBinding(1414,true)
            .WithPortBinding(9443,true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1414))
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(9443))
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*(Started queue manager)"))
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*(Started web server)"))
            .WithImage("icr.io/ibm-messaging/mq:latest");

        if (network is not null)
        {
            containerBuilder = containerBuilder.WithNetwork(network);
        }
        container = containerBuilder.Build();
    }
}
