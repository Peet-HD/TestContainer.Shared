namespace TestContainer.Shared.Models.Gitlab;

public record PersonalAccessToken
{
    public string Name { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public PersonalAccessTokenScopes Scope { get; set; } = PersonalAccessTokenScopes.None;
    public int ExpirationInDays { get; set; } = 365;
    internal string TokenInternal { get; set; } = string.Empty;
    public string Token => TokenInternal;
}

[Flags]
public enum PersonalAccessTokenScopes
{
    None = 0,
    api = 2,
    read_api = 4,
    read_user = 8,
    read_repository = 16,
    write_repository = 32,
    read_registry = 64,
    write_registry = 128,
    create_runner = 256,
    ai_features = 512,
    k8s_proxy = 1024,
}
