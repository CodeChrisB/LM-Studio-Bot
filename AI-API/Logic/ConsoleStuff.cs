using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AI_API.Models;

namespace AI_API.Logic
{
    internal class ConsoleStuff
    {
        public delegate void BeforePrint();
        public delegate void AfterPrint();
        public static void ConsoleWriteLineColor(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ResetColor();

        }
        public static void LogStatus(ref bool hadTimeout, string[] chunks, ChatResponse lastCorrected, int i, bool done,BeforePrint bp = null, AfterPrint ap = null)
        {
            Console.Clear();
            if (bp != null) bp();
            if (!done)
            {
                if (hadTimeout) ConsoleWriteLineColor(ConsoleColor.Red,"==  => We had a timeout");
                hadTimeout = false;
                ConsoleWriteLineColor(ConsoleColor.Green, $"===> Text Chunk {i + 1} of {chunks.Length} {GenerateLoadingBar(i, chunks.Length, 20)}");
                Console.WriteLine(chunks[i]);
            }
            else
            {
                ConsoleWriteLineColor(ConsoleColor.Green, $"===> [Done {chunks.Length}/{chunks.Length}] {GenerateLoadingBar(1, 1, 20)}");
            }


            if (i > 0)
            {
                ConsoleWriteLineColor(ConsoleColor.Green, $"===> Last Chunk [Tokens:{lastCorrected.Usage.CompletionTokens}]");
                Console.WriteLine(lastCorrected.ToString());
            }

            if (ap != null) ap();

        }
        public static string[] SplitTextIntoChunks(string text, int lengthInSentences)
        {
            // Split the text into sentences
            string[] sentences = text.Split(new[] { ". " }, StringSplitOptions.None);
            List<string> chunks = new List<string>();

            for (int i = 0; i < sentences.Length; i += lengthInSentences)
            {
                // Select a chunk of sentences
                var chunkSentences = new List<string>();
                for (int j = i; j < i + lengthInSentences && j < sentences.Length; j++)
                {
                    chunkSentences.Add(sentences[j].Trim() + (j < sentences.Length - 1 ? ". " : "."));
                }
                // Join the sentences into a single string
                chunks.Add(string.Join("", chunkSentences).Trim());
            }

            return chunks.ToArray();
        }

        public static string GenerateLoadingBar(double currentValue, double maxValue, int barLength)
        {
            char[] progressChars = { '░', '▒', '▓', '█' };
            if (maxValue <= 0 || barLength <= 0 || currentValue < 0)
            {
                return new string(progressChars[0], barLength);
            }

            double percentage = (double)currentValue / maxValue;
            int filledLength = (int)(percentage * barLength * (progressChars.Length - 1));
            int fullBlocks = filledLength / (progressChars.Length - 1);
            int remainder = filledLength % (progressChars.Length - 1);

            string bar = new string(progressChars[^1], fullBlocks);
            if (fullBlocks < barLength)
            {
                bar += progressChars[remainder];
                bar += new string(progressChars[0], barLength - fullBlocks - 1);
            }


            return $"{bar} ({percentage * 100:0}%)";
        }
    }
}
