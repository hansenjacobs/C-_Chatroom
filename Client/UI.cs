using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class UI
    {
        public static void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }
        public static string GetInput()
        {
            string input = Console.ReadLine();
            int inputLineEnd = Console.CursorTop;

            Console.SetCursorPosition(0, inputLineEnd - 1);

            return input;
        }
    }
}
