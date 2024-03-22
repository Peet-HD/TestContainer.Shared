using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using TestContainer.Shared.Containers.RegexPatterns;
using TestContainer.Shared.Models;
using TestContainer.Shared.Models.Gitlab;

namespace TestContainer.Shared.Containers;

public partial class GitlabContainer : BaseContainer
{
    public List<PersonalAccessToken> PersonalAccessTokens { get; set; } = [];
    public string RootPassword { get; private set; } = string.Empty;

    public GitlabContainer()
    {
        var containerName = "TestContainer_Gitlab_" + Guid.NewGuid().ToString();
        container = new ContainerBuilder()
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
    }
    public async Task<PersonalAccessToken> GenerateAccessToken(PersonalAccessToken pat)
    {
        var scope = "[" + '\'' + pat.Scope.ToString().Replace(", ", "\', \'") + '\'' + "]";

        var variable = "token";

        ExecResult tokenResult = await ExecGitlabRailsAsync($"{variable} = User.find_by_username('{pat.User}')" +
            $".personal_access_tokens" +
            $".create(name: '{pat.Name}', scopes: {scope}, expires_at: {pat.ExpirationInDays}.days.from_now); " +
            $"puts {variable}.cleartext_tokens");

        string? token;
        if (tokenResult.ExitCode == 0)
        {
            var match = GitlabTokens.GitlabPersonalAccessToken.Match(tokenResult.Stdout);
            token = match.Value;
        }
        else
        {
            throw new DataMisalignedException("Stderr: " + tokenResult.Stderr + "|" + "Stdout: " + tokenResult.Stdout);
        }
        pat.TokenInternal = token;
        PersonalAccessTokens.Add(pat);
        return pat;
    }

    private async Task<ExecResult> ExecGitlabRailsAsync(string command)
    {
        var tokenShowCommands = new List<string>{
            { "gitlab-rails" },
            { "runner" },
            { command }
        };
        return await container.ExecAsync(tokenShowCommands);
    }

    private async Task<string> RetrieveRootPassword()
    {
        var byteArray = await container.ReadFileAsync("/etc/gitlab/initial_root_password");

        string fileContent = Encoding.UTF8.GetString(byteArray);
        string pattern = @"Password: .*";

        RegexOptions options = RegexOptions.IgnoreCase;
        var match = Regex.Match(fileContent, pattern, options);
        var splits = match.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        RootPassword = splits[1];
        return splits[1];
    }

    public new async Task StartAsync(CancellationToken ct = default)
    {
        RootPassword = await RetrieveRootPassword();
        await base.StartAsync(ct);
    }
}