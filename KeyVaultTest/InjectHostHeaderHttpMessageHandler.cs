using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace KeyVaultTest
{
    public class InjectHostHeaderHttpMessageHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken)
                .ContinueWith(response =>
            {
                return response.Result;
            });
        }
    }
}
