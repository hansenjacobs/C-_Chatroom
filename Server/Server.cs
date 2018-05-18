using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        public static Client client;
        TcpListener server;
        Queue<byte[]> messageQueue;
        Dictionary <string, Client> clients;
        
        public Server()
        {
            server = new TcpListener(IPAddress.Parse("127.0.0.1"), 9999);
            server.Start();
            messageQueue = new Queue<byte[]>();
            clients = new Dictionary<string, Client>();
        }
        
        public void Run()
        {
            Parallel.Invoke(
            ()=>
            {
                AcceptClient();
            },
            ()=>
            {
                DeliverQueueMessages();
            }
            );
            
        }
        
        private void AcceptClient()
        {
            while (true)
            { 
                    //find better condition later 
                TcpClient clientSocket = default(TcpClient);
                clientSocket = server.AcceptTcpClient();
                
                Console.WriteLine("Connected");
                NetworkStream stream = clientSocket.GetStream();
                clients.Add(clientSocket.Client.RemoteEndPoint.ToString(), new Client(stream, clientSocket, messageQueue));
                Parallel.Invoke(()=> 
                {
                    clients[clientSocket.Client.RemoteEndPoint.ToString()].Receive();
                }
                    );
            }
        }

        private void DeliverQueueMessages()
        {
            while (true)
            {
                if (CheckQueue())
                {
                    Respond(messageQueue.Dequeue());
                }
            }
        }
        private void Respond(byte[] body)
        {
             client.Send(body);
        }

        private bool CheckQueue()
        {
            if (messageQueue.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }  
        }

        //private void Chat(Server.Respond, Client.Send)
    }
}
