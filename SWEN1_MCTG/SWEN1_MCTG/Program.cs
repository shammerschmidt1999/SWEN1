using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.ComponentModel.Design;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Classes.HttpSvr;
using System.Text.Json;
using SWEN1_MCTG.Classes.HttpSvr.Handlers;

namespace SWEN1_MCTG
{
    internal class Program
    {
        public const bool ALLOW_DEBUG_TOKEN = true;

        static void Main(string[] args)
        {
            HttpSvr svr = new();
            svr.Incoming += Svr_Incoming;

            svr.Run();
        }

        private static void Svr_Incoming(object sender, HttpSvrEventArgs e)
        {
            Console.WriteLine(e.Method);
            Console.WriteLine(e.Path);
            Console.WriteLine();
            foreach (HttpHeader i in e.Headers)
            {
                Console.WriteLine(i.Name + ": " + i.Value);
            }

            if (e.Path.Contains("/users"))
            {

                UserHandler userHandler = new();

                try
                {
                    userHandler.Handle(e);
                }
                catch (Exception ex)
                {
                    e.Reply(HttpStatusCode.BAD_REQUEST, $"Error processing request: {ex.Message}");
                }
            }

            else if (e.Path.Contains("/sessions"))
            {
                SessionHandler sessionHandler = new();

                try
                {
                    sessionHandler.Handle(e);
                }
                catch (Exception ex)
                {
                    e.Reply(HttpStatusCode.BAD_REQUEST, $"Error processing request: {ex.Message}");
                }
            }
            else
            {
                e.Reply(HttpStatusCode.NOT_FOUND, "Endpoint not found");
            }
        }
    }
}
