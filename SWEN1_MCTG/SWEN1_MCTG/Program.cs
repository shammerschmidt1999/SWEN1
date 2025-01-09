using SWEN1_MCTG.Classes;
using SWEN1_MCTG.Classes.HttpSvr;
using SWEN1_MCTG.Classes.HttpSvr.Handlers;

namespace SWEN1_MCTG
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = AppSettings.GetConnectionString("TestConnection");

            // Initialize repositories
            ITokenRepository tokenRepository = new TokenRepository(connectionString);

            // Initialize Token class with the repository
            Token.Initialize(tokenRepository);

            HttpSvr svr = new();
            svr.Incoming += Svr_Incoming;

            svr.Run();
        }

        private static async void Svr_Incoming(object sender, HttpSvrEventArgs e)
        {
            Console.WriteLine(e.Method);
            Console.WriteLine(e.Path);
            Console.WriteLine();
            foreach (HttpHeader i in e.Headers)
            {
                Console.WriteLine(i.Name + ": " + i.Value);
            }

            await Handler.HandleEventAsync(e);
        }
    }
}
