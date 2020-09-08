using System.Net;
using System.Threading;
using System.Threading.Tasks;

public static class WebClientExtensions
{
    public static async Task<byte[]> DownloadDataTaskAsyncCancellable(this WebClient client, string url, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        using (token.Register(client.CancelAsync))
        {
            return await client.DownloadDataTaskAsync(url);
        }
    }
}