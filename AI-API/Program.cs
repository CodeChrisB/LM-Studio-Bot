using System;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection.Metadata;
using System.Diagnostics;
using System.Net;
using AI_API.Logic;
using AI_API;

class Program
{
    static void Main(string[] args)
    {

        AiServer aiServer = new AiServer(
            "http://localhost:1234/v1",
            "lm-studio",
            "mayflowergmbh/Llama3_DiscoLM_German_8b_v0.1_experimental-GGUF"
            );


        string[] sysPrompt = new string[] {
            "Your only goal is to grammatically and spellcheck the users text." ,
            "Your only response will be either the users text or the corrected text that you corrected.",
            "Also add punctuation marks where needed",
            "The text will be in german. Your corrected text will also be in german.",
            "You only responed with the corrected text no information about what you corrected or why you did it"
        };


        //read file
        string text = File.ReadAllText("text.txt");
        string[] chunks = ConsoleStuff.SplitTextIntoChunks(text,5);
        string correctedText = "";
        string lastCorrected = "";
        bool hadTimeout = false;
        for(int i=0; i<chunks.Length; i++)
        {

            ConsoleStuff.LogStatus(ref hadTimeout, chunks, lastCorrected, i,false);
            hadTimeout = false;


            AiPayload aiPayload = aiServer.GetPayloadGenerator()
                .AddSystemMessage(string.Join(' ',sysPrompt))
                .AddUserMessage(chunks[i])
                .GeneratePayload();

            dynamic response = aiServer.ExecutePayload(aiPayload);

            if(response.StatusCode != HttpStatusCode.OK)
            {
                i--;
                hadTimeout = true;
                continue;
            }
           
            var jsonResponse = JObject.Parse(response.Content);
            var message = jsonResponse["choices"][0]["message"]["content"].ToString();

            lastCorrected = message;
            correctedText += message;
        }

        ConsoleStuff.LogStatus(ref hadTimeout, chunks, lastCorrected, chunks.Length,true);

        File.WriteAllText("corrected.txt", correctedText);
        ConsoleStuff.ConsoleWriteLineColor(ConsoleColor.Green,"> Saved corrected.txt") ;
    }
}
