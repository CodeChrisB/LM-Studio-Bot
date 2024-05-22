using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AI_API
{
    internal class PayloadGenerator
    {
        private string model;
        private List<AiMessage> messages = new List<AiMessage>();
        private double temperature = 0.7;
        private int maxTokens = 10_000;
        public PayloadGenerator(string model)
        {
            this.model = model;
        }

        public AiPayload GeneratePayload() {
            return new AiPayload(
                model,
                messages.ToArray(),
                temperature,
                maxTokens
            );
        }

        public PayloadGenerator Flush()
        {
            messages = new List<AiMessage>();
            temperature = 0.7;
            maxTokens = 10_000;
            return this;
        }
        public void SetMaxTokens(int max) => maxTokens = max;
        public void SetTemperature(double temp) => temperature = temp;
        public PayloadGenerator AddUserMessage(string content) => AddMessage("user", content);
        public PayloadGenerator AddUserMessage(string[] content) => AddMessage("user", string.Join(' ', content));
        public PayloadGenerator AddSystemMessage(string content) => AddMessage("system", content);
        public PayloadGenerator AddSystemMessage(string[] content) => AddMessage("system", string.Join(' ', content));
        private PayloadGenerator AddMessage(string user, string content) {
            messages.Add(new AiMessage(user,content));
            return this;
        }


    }
}
