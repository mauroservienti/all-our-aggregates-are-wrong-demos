using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

namespace JsonUtils
{
    public static class HttpContentExtensions
    {
        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new PascalCaseExpandoObjectConverter() }
        };

        public static async Task<T> As<T>(this HttpContent content)
            => JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync(), CamelCaseToPascalSettings.GetSerializerSettings());

        public static async Task<ExpandoObject> AsExpando(this HttpContent content)
            => JsonConvert.DeserializeObject<ExpandoObject>(await content.ReadAsStringAsync(), serializerSettings);

        public static async Task<ExpandoObject[]> AsExpandoArray(this HttpContent content)
            => JsonConvert.DeserializeObject<ExpandoObject[]>(await content.ReadAsStringAsync(), serializerSettings);
    }
}