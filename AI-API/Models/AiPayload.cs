using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AI_API
{
    public class AiPayload
    {
        public string model;
        public AiMessage[] messages;
        public double temperature;
        public int max_tokens = 10_000;

        public AiPayload(string model, AiMessage[] messages, double temperature,int max)
        {
            this.model = model;
            this.messages = messages;
            this.temperature = temperature;
            this.max_tokens = max;
        }   

        public string GetJsonString() {
            return JsonConvert.SerializeObject(this);
        }
    }
}
