using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Networks;
using System.Text.Json;
using TestContainer.Shared.Extensions;
using TestContainer.Shared.Models;
using TestContainer.Shared.Models.Sonarqube;
using TestContainer.Shared.Options;

namespace TestContainer.Shared.Containers;

public class SonarqubeContainer : BaseContainer
{
    public string UserToken { get; private set; } = string.Empty;
    public string AnalysisToken { get; private set; } = string.Empty;
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
        container = containerbuilder.Build();
    }

    private static async Task<AnalyseTokenType?> GetToken(int port, string tokenName, string type)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:{port}/api/user_tokens/generate?name={tokenName}&type={type}");

        request.AddBasicAuthentication("root", "password");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var resultString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<AnalyseTokenType>(resultString, GlobalOptions.JsonSerializerOptions);
        return result;
    }

    public new async Task StartAsync(CancellationToken ct = default)
    {
        await container.StartAsync(ct);

        AnalyseTokenType? analysisToken = await GetToken(container.GetMappedPublicPort(9000), "analyseToken", "GLOBAL_ANALYSIS_TOKEN");
        AnalyseTokenType? userToken = await GetToken(container.GetMappedPublicPort(9000), "userToken", "USER_TOKEN");

        if (analysisToken is not null) AnalysisToken = analysisToken.Token;
        if (userToken is not null) UserToken = userToken.Token;
    }
}