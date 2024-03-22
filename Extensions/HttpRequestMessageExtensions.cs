using System.Text;

namespace TestContainer.Shared.Extensions;

internal static class HttpRequestMessageExtensions
{
    public static void AddBasicAuthentication(this HttpRequestMessage message, string user, string password)
    {
        var basicAuth = $"{user}:{password}";
        var basicAuthBase64 = Encoding.ASCII.GetBytes(basicAuth);
        var authentication = Convert.ToBase64String(basicAuthBase64);
        message.Headers.Add("Authorization", authentication);
    }
}
