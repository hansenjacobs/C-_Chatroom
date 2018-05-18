using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Client");
            Client client = new Client("192.168.0.102", 9999);
            Parallel.Invoke(
            () =>
            {
                client.Send();
            },
            () =>
            {
                client.Recieve();
            }
            );

            Console.ReadLine();
        }
    }
}
