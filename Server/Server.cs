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
        TcpListener server;
        Queue<Message> messageQueue;
        Dictionary <string, Client> clients;
        
        public Server()
        {//"192.168.0.102" -- Jacob
         //"127.0.0.1", 8888 -- default when same machine
            server = new TcpListener(IPAddress.Parse("192.168.0.111"), 8888);
            server.Start();
            messageQueue = new Queue<Message>();
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
                Client client = clients[clientSocket.Client.RemoteEndPoint.ToString()];
                Thread clientReceive = new Thread(new ThreadStart(client.Receive));
                clientReceive.Start();
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
        private void Respond(Message message)
        {
            foreach(KeyValuePair<string, Client> pair in clients)
            {
                pair.Value.Send(message.EncodedMessage);
            }
             
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
    }
}
