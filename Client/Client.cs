using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        TcpClient clientSocket;
        NetworkStream stream;
        public Client(string IP, int port)
        {
            clientSocket = new TcpClient();
            try
            {
                clientSocket.Connect(IPAddress.Parse(IP), port);
                stream = clientSocket.GetStream();
            }
            catch (SocketException e)
            {
                Console.WriteLine("Unable to connect to the server.");
                Console.WriteLine(e.Message + "\n");
                Console.WriteLine("Please close and try again");
            }

        }
        public void Send()
        {
            while (true)
            {
                string messageString = UI.GetInput();
                byte[] message = Encoding.ASCII.GetBytes(messageString);
                stream.Write(message, 0, message.Count());
            }
        }
        public void Recieve()
        {
            while (true)
            {
                byte[] recievedMessage = new byte[5000];
                stream.Read(recievedMessage, 0, recievedMessage.Length);
                UI.DisplayMessage(Encoding.ASCII.GetString(recievedMessage).Trim('\0'));
            }
        }


    }
}
