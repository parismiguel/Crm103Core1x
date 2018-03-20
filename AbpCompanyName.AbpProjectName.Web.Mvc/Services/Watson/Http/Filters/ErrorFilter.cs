using System.Net.Http;
using IBM.VCA.Watson.Watson.Http.Exceptions;
using Newtonsoft.Json;

namespace IBM.VCA.Watson.Watson.Http.Filters
{
    public class ErrorFilter : IHttpFilter
    {
        public void OnRequest(IRequest request, HttpRequestMessage requestMessage) { }

        public void OnResponse(IResponse response, HttpResponseMessage responseMessage)
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                ServiceResponseException exception =
                    new ServiceResponseException(response, responseMessage, $"The API query failed with status code {responseMessage.StatusCode}: {responseMessage.ReasonPhrase}");

                var jsonError = responseMessage.Content.ReadAsStringAsync().Result;

                exception.Error = JsonConvert.DeserializeObject<Error>(jsonError);

                throw exception;
            }
                
        }
    }
}