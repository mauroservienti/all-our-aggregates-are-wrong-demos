using Newtonsoft.Json;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;

namespace JsonUtils
{
    public static class BodyStreamExtensions
    {
        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new PascalCaseExpandoObjectConverter() }
        };

        public static ExpandoObject AsExpando(this Stream body)
        {
            using (var readStream = new StreamReader(stream: body, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, encoding: Encoding.UTF8, leaveOpen: true))
            {
                var documentContents = readStream.ReadToEnd();
                body.Position = 0;
                return JsonConvert.DeserializeObject<ExpandoObject>(documentContents, serializerSettings);
            }
        }

        public static ExpandoObject[] AsExpandoArray(this Stream body)
        {
            using (var readStream = new StreamReader(stream: body, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, encoding: Encoding.UTF8, leaveOpen: true))
            {
                var documentContents = readStream.ReadToEnd();
                body.Position = 0;
                return JsonConvert.DeserializeObject<ExpandoObject[]>(documentContents, serializerSettings);
            }
        }
    }
}
