using System;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection.Metadata;
using System.Diagnostics;
using System.Net;
using AI_API.Logic;
using AI_API;
using AI_API.Models;

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
            "Your only goal is to grammatically and spellcheck the user's text.",
            "Your only response will be either the user's text or the corrected text.",
            "Also add punctuation marks where needed.",
            "The text will be in German. Your corrected text will also be in German.",
            "You are a German professor and will correct the text at a university level, but you will not change the intent of the text, only correct all types of mistakes.",
            "You will also correct punctuation mistakes.",
            "You will only respond with the corrected text, without any explanation of the corrections."
        };


        //read file
        string text = File.ReadAllText("text.txt");
        string[] chunks = ConsoleStuff.SplitTextIntoChunks(text,5);
        string correctedText = "";

        bool hadTimeout = false;

        for(int i=0; i<chunks.Length; i++)
        {
            ConsoleStuff.LogStatus(ref hadTimeout, chunks, aiServer.LastResponseMessage, i,false);
            hadTimeout = false;

            AiPayload aiPayload = aiServer
                .GetPayloadGenerator()
                .AddSystemMessage(sysPrompt)
                .AddUserMessage(chunks[i])
                .GeneratePayload();

            ChatResponse response = aiServer.ExecutePayload(aiPayload);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                //Dirty way to retry current chunk
                i--;
                hadTimeout = true;
                continue;
            }

            var message = response.ToString();
            correctedText += message;
        }

        ConsoleStuff.LogStatus(ref hadTimeout, chunks, aiServer.LastResponseMessage, chunks.Length,true);

        File.WriteAllText("corrected.txt", correctedText);
        ConsoleStuff.ConsoleWriteLineColor(ConsoleColor.Green,"> Saved corrected.txt") ;        
    }
}
