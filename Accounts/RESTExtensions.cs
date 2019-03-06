using RestSharp;

namespace Accounts
{
    public static class RestRequestExetensions
    {
        public static void SetJsonContentType(this RestRequest restRequest)
        {
            restRequest.AddHeader("Content-Type", "application/json");
        }
    }
}
