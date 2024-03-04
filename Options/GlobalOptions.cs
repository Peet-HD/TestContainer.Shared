using System.Text.Json;

namespace TestContainer.Shared.Options;

internal static class GlobalOptions
{
    internal static JsonSerializerOptions JsonSerializerOptions => new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };
}
