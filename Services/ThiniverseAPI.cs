using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using thingiverseCLI.Commands;
using thingiverseCLI.Models;

namespace thingiverseCLI.Services
{
    public class ThingiverseAPI
    {
        private readonly HttpClient client;
        private readonly IConfiguration configuration;
        private readonly string apiKey;

        private string AccessParameter { get { return $"?access_token={this.apiKey}"; } }

        public ThingiverseAPI(HttpClient client, IConfiguration configuration)
        {
            this.client = client;
            this.configuration = configuration;
            this.apiKey = configuration.GetValue<string>(Config.APIKeyEnvironmentVarName);
            this.client.BaseAddress = new System.Uri(@"https://api.thingiverse.com/");
        }

        public async Task<IEnumerable<ThingFile>> GetFiles(int thingId)
        {
            return await this.client.GetFromJsonAsync<IEnumerable<ThingFile>>($"/things/{thingId}/files{this.AccessParameter}");
        }
    }
}