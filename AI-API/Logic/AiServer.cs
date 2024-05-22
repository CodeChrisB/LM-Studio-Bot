using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI_API.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace AI_API
{
    internal class AiServer
    {        
        // Define the API endpoint and key
        string baseUrl = "http://localhost:1234/v1";
        string apiKey = "lm-studio";
        string model = "mayflowergmbh/Llama3_DiscoLM_German_8b_v0.1_experimental-GGUF";

        private RestClient client;
        private RestRequest request;
        private List<ChatResponse> responsList = new List<ChatResponse>();

        public ChatResponse? LastResponse
        {
            get {
                if (responsList.Count == 0) return null;
                return responsList[0];
            }
        }
        public string LastResponseMessage
        {
            get
            {
                if (LastResponse == null) return "";
                return LastResponse.ToString();
            }
        }
        public AiServer(string baseUrl, string apiKey, string model)
        {
            this.baseUrl = baseUrl;
            this.apiKey = apiKey;
            this.model = model;

            // Create a RestClient
            client = new RestClient(baseUrl);

            // Create a request for chat completions
            request = new RestRequest("chat/completions", Method.Post);
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("Content-Type", "application/json");
        }

        private void CreateRequest()
        {
            request = new RestRequest("chat/completions", Method.Post);
            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("Content-Type", "application/json");
        }

        public void AddPayload(AiPayload aiPayload)
        {
            CreateRequest();
            request.AddJsonBody(aiPayload.GetJsonString());
        }

        public ChatResponse ExecutePayload(AiPayload aiPayload)
        {
            AddPayload(aiPayload);
            return ExecuteRequest();
        }

        public ChatResponse ExecuteRequest()
        {
            dynamic response = client.Execute(request);

            ChatResponse chatResponse = JsonConvert.DeserializeObject<ChatResponse>(response.Content);
            chatResponse.StatusCode = response.StatusCode;

            responsList.Insert(0,chatResponse);
            if (responsList.Count > 2) responsList.RemoveAt(responsList.Count-1);
  
            return chatResponse;
        }

        public PayloadGenerator GetPayloadGenerator()
        {
            return new PayloadGenerator(model);
        }
    }
}
