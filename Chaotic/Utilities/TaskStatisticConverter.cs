using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chaotic.Tasks;
using Chaotic.Tasks.Chaos;

namespace Chaotic.Utilities
{
    public class TaskStatisticItemConverter : Newtonsoft.Json.Converters.CustomCreationConverter<ITaskStatistic>
    {
        public override ITaskStatistic Create(Type objectType)
        {
            throw new NotImplementedException();
        }

        public ITaskStatistic Create(Type objectType, JObject jObject)
        {
            var type = (string)jObject.Property("StatisticType");

            switch (type)
            {
                case "ChaosDungeon":
                    return new ChaosTaskStatistic();
                case "Kurzan":
                    return new KurzanTaskStatistic();
                default:
                    return new TaskStatistic();
            }

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load JObject from stream 
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject 
            var target = Create(objectType, jObject);

            // Populate the object properties 
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }
    }
}
