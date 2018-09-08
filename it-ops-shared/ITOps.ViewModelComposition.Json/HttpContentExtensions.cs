using Newtonsoft.Json;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition.Json
{
    public static class HttpContentExtensions
    {
        public static async Task<T> As<T>(this HttpContent content)
            => JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync(), CamelCaseToPascalSettings.GetSerializerSettings());

        public static async Task<ExpandoObject> AsExpando(this HttpContent content)
            => await As<ExpandoObject>(content);

        public static async Task<ExpandoObject[]> AsExpandoArray(this HttpContent content)
            => await As<ExpandoObject[]>(content);
    }
}