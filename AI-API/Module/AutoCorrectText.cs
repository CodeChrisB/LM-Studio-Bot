using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using AI_API.Logic;
using AI_API.Models;

namespace AI_API.Module
{
    public  class AutoCorrectText
    {
        public static void Correct()
        {
            CorrectFile("text.txt","corrected.txt");
        }

        public static void CorrectFolder()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                if(!Directory.Exists("text")) Directory.CreateDirectory("text");
                if(!Directory.Exists("corrected")) Directory.CreateDirectory("corrected");
                // Get all .txt files in the specified folder
                string[] txtFiles = Directory.GetFiles("text", "*.txt");

                // Check if there are any .txt files found
                if (txtFiles.Length == 0)
                {
                    Console.WriteLine("No .txt files found in the specified folder.");
                }
                else
                {
                    Console.WriteLine("List of .txt files:");
                    for (int i = 0; i < txtFiles.Length; i++)
                    {
                        string file = txtFiles[i];
                        // Extract and print the file name
                        CorrectFile(file, $"corrected//{file.Split("\\")[1]}",i+1,txtFiles.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                TimeSpan elapsedTime = stopwatch.Elapsed;
                Console.WriteLine("Elapsed time: " + elapsedTime);
            }

        }
        private static void OverallStatus(int _current,int _overall)
        {
            if (_current == -1 || _overall == -1) return;
            //╔═╗║╚═╝
            string statusText = $"==> File: {_current}/{_overall}";
            int statusLength = statusText.Length;
            int offset = statusLength > 20 ? 0 : 20-statusLength;
            statusLength = (statusLength >= 20 ? statusLength : 20);

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"╔{new string('═',statusLength)}╗");
            sb.AppendLine($"║{statusText}{new string(' ', offset)}║"); 
            sb.AppendLine($"║{ConsoleStuff.GenerateLoadingBar(_current, _overall, 15)}║");
            sb.AppendLine($"╚{new string('═', statusLength)}╝");

            //╔═╗
            ConsoleStuff.ConsoleWriteLineColor(ConsoleColor.Green, sb.ToString());
        }
        private static void CorrectFile(string path,string outputFile,int _current = -1, int _overall = -1)
        {
            AiServer aiServer = new AiServer(
            "http://localhost:1234/v1",
            "lm-studio",
            "mayflowergmbh/Llama3_DiscoLM_German_8b_v0.1_experimental-GGUF"
            );

            string[] sysPrompt = new string[] {
                "Your task: Correct German text for grammar, spelling, and readability.",
                "It is extremly important to not change the language, intent and meaning of the text just fix spelling, grammar and punctations",
                "Ensure the corrected text is in clear and understandable German.",
                "Maintain consistent style and tone throughout the German text.",
                "Avoid using whole English sentences; use English words only if present in the provided text.",
                "Avoid overly complex or convoluted sentences; aim for simplicity.",
                "Clarify phrases to improve comprehension.",
                "Check for any remaining punctuation errors or awkward phrasing.",
                "Follow these additional rules to improve readability and language usage:",
                "Language: Generate text exclusively in clear and correct German.",
                "Sentence Structure: Ensure sentences are concise and clear.",
                "Clarity: Clarify ambiguous or unclear phrases.",
                "Tone: Maintain a consistent tone throughout the German text.",
                "Coherence: Ensure ideas flow logically from one to another.",
                "Readability: Aim for text that is easy to read and understand.",
                "Correct any remaining punctuation errors or awkward phrasing."
            };


            //read file
            string text = File.ReadAllText(path);
            string[] chunks = ConsoleStuff.SplitTextIntoChunks(text, 5);
            string correctedText = "";

            bool hadTimeout = false;

            for (int i = 0; i < chunks.Length; i++)
            {
                ConsoleStuff.LogStatus(ref hadTimeout, chunks, aiServer.LastResponse, i, false, () => OverallStatus(_current,_overall));
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

            ConsoleStuff.LogStatus(ref hadTimeout, chunks, aiServer.LastResponse, chunks.Length, true);

            File.WriteAllText(outputFile, correctedText);
            ConsoleStuff.ConsoleWriteLineColor(ConsoleColor.Green, "> Saved corrected.txt");
        }
    }
}
