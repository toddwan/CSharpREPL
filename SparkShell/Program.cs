using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Reflection;
using Newtonsoft.Json;

namespace SparkShell
{
    public class HostObject
    {
        public string hostStr = "This is host string.";
        public int port = 3306;
    }

    class Program
    {

        // Microsoft.CodeAnalysis.Scripting/Hosting/CommandLine/CommandLineRunner.cs might also a good choice.
        // Also checkout ObjectFormatter.cs in Roslyn

        static void Main(string[] args)
        {
        Console.WriteLine("Welcome to SparkCLR shell.");

            var options = ScriptOptions.Default
                .AddReferences(typeof(JsonConvert).GetTypeInfo().Assembly)
                .AddReferences(typeof(HostObject).GetTypeInfo().Assembly)
                .AddImports("SparkShell");

            var hostObject = new HostObject();

            var scriptState = CSharpScript.Create(@"
using System;
using Newtonsoft.Json;
using SparkShell;

Console.Write("">> "");
string json = JsonConvert.SerializeObject(""json"");
Console.WriteLine(""json"" + json);
", options, typeof(HostObject)).RunAsync(hostObject).Result;


            while(true)
            {
                var code = Console.ReadLine();
                if (code.Equals("quit();", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }

                try
                {
                    scriptState = scriptState.ContinueWithAsync(code, ScriptOptions.Default).Result;
                    var response = scriptState.ReturnValue;
                    Console.Write(response);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }

                Console.WriteLine();
                Console.Write(">> ");
            }

        }
    }
}
