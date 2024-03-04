using System.ComponentModel;
using System.Text.RegularExpressions;

namespace TestContainer.Shared.Containers.RegexPatterns;
public static partial class GitlabTokens
{
    [Description("GitLab Personal Access Token")]
    [GeneratedRegex(@"glpat-[0-9a-zA-Z_\-]{20}")]
    private static partial Regex gitlabPersonalAccessToken();
    public static Regex GitlabPersonalAccessToken => gitlabPersonalAccessToken();

    [Description("GitLab Pipeline Trigger Token")]
    [GeneratedRegex(@"glptt-[0-9a-zA-Z_\-]{40}")]
    private static partial Regex gitlabPipelineTriggerToken();
    public static Regex GitlabPipelineTriggerToken => gitlabPipelineTriggerToken();
    [Description("GitLab Runner Registration Token")]
    [GeneratedRegex(@"GR1348941[0-9a-zA-Z_\-]{20}")]
    private static partial Regex gitlabRunnerRegistrationToken(); 
    public static Regex GitlabRunnerRegistrationToken => gitlabRunnerRegistrationToken();
    [Description("GitLab Runner Authentication Token")]
    [GeneratedRegex(@"glrt-[0-9a-zA-Z_\-]{20}")]
    private static partial Regex gitlabRunnerAuthenticationToken();
    public static Regex GitlabRunnerAuthenticationToken => gitlabRunnerAuthenticationToken();
    [Description("GitLab Feed Token")]
    [GeneratedRegex(@"feed_token=[0-9a-zA-Z_\-]{20}")]
    private static partial Regex gitlabFeedToken();
    public static Regex GitlabFeedToken => gitlabFeedToken();

    [Description("GitLab OAuth Application Secrets")]
    [GeneratedRegex(@"gloas-[0-9a-zA-Z_\-]{64}")]
    private static partial Regex gitlabOauthApplicationToken();
    public static Regex GitlabOauthApplicationToken => gitlabOauthApplicationToken();

    [Description("GitLab Feed token v2")]
    [GeneratedRegex(@"glft-[0-9a-zA-Z_\-]{20}")]
    private static partial Regex gitlabFeedTokenV2();
    public static Regex GitlabFeedTokenV2 => gitlabFeedTokenV2();
    [Description("GitLab Agent for Kubernetes token")]
    [GeneratedRegex(@"glagent-[0-9a-zA-Z_\-]{50}")]
    private static partial Regex gitlabK8sAgentToken();
    public static Regex GitlabK8sAgentToken => gitlabK8sAgentToken();
    [Description("GitLab Incoming email token")]
    [GeneratedRegex(@"glimt-[0-9a-zA-Z_\-]{25}")]
    private static partial Regex gitlabIncomingEmailToken();
    public static Regex GitlabIncomingEmailToken => gitlabIncomingEmailToken();
    [Description("GitLab Deploy Token")]
    [GeneratedRegex(@"gldt-[0-9a-zA-Z_\-]{20}")]
    private static partial Regex gitlabDeployToken();
    public static Regex GitlabDeployToken => gitlabDeployToken();
    [Description("GitLab SCIM token")]
    [GeneratedRegex(@"glsoat-[0-9a-zA-Z_\-]{20}")]
    private static partial Regex gitlabScimToken();
    public static Regex GitlabScimToken => gitlabScimToken();
    [Description("GitLab CI Build (Job) token")]
    [GeneratedRegex(@"glcbt-[0-9a-zA-Z]{1,5}_[0-9a-zA-Z_-]{20}")]
    private static partial Regex gitlabCiBuildJobToken();
    public static Regex GitlabCiBuildJobToken => gitlabCiBuildJobToken();
}