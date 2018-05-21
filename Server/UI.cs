using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public static class UI
    {
        public static bool ServerRunning { get; set; } = true;

        public static string GetUserInput()
        {
            return Console.ReadLine();
        }

        public static void AwaitCommand()
        {

            while (ServerRunning)
            {
                string command = GetUserInput();

                switch (command)
                {
                    case "quit":
                        ServerRunning = false;
                        break;

                    default:
                        Console.WriteLine("Invlid command.");
                        break;
                }
            }
        }
    }
}
