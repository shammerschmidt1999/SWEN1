using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Classes.HttpSvr;

namespace SWEN1_MCTG
{
    internal class Program
    {
        // To start, press play, open another terminal, get curl script from Moodle, paste under #create user, server is empty console,
        // paste curl script into second terminal with correct data (Port)
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
            Console.WriteLine();
            Console.WriteLine(e.Payload);

            e.Reply(HttpStatusCode.OK, "Yo Baby!");
        }
    }
}
