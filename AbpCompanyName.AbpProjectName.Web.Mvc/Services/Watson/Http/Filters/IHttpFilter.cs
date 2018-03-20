
using System.Net.Http;

namespace IBM.VCA.Watson.Watson.Http.Filters
{
    public interface IHttpFilter
    {
        void OnRequest(IRequest request, HttpRequestMessage requestMessage);

        void OnResponse(IResponse response, HttpResponseMessage responseMessage);
    }
}