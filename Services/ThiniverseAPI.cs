using System.Net.Http;

namespace thingiverseCLI.Services
{
    public class ThingiverseAPI
    {
        private readonly HttpClient client;

        public ThingiverseAPI(HttpClient client)
        {
            this.client = client;
        }
    }
}