using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DrMarioPlayer.Converter
{
    internal class JsonPathConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            object targetObj = Activator.CreateInstance(objectType);

            foreach (PropertyInfo prop in objectType.GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                JsonPropertyAttribute att = prop.GetCustomAttributes(true).OfType<JsonPropertyAttribute>().FirstOrDefault();

                string jsonPath = (att != null ? att.PropertyName : prop.Name);
                JToken token = jo.SelectToken(jsonPath);

                if(token != null && token.Type != JTokenType.Null)
                {
                    object value = token.ToObject(prop.PropertyType, serializer);
                    prop.SetValue(targetObj, value, null);
                }
            }

            return targetObj;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
