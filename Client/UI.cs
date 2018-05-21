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
            int inputLineStart = Console.CursorTop;
            string input = Console.ReadLine();
            int inputLineEnd = Console.CursorTop;

            for(int i = 0; i < inputLineEnd - inputLineStart; i++)
            {
                Console.CursorTop = inputLineStart;
                Console.WriteLine("".PadRight(Console.BufferWidth));
            }

            Console.CursorTop = inputLineStart;

            return input;
        }
    }
}
