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
using AI_API.Module;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        CCModules.CorrectFolder();
    }
}
