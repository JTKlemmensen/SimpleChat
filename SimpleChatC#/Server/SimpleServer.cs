using Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class SimpleServer
    {
        private int port;
        private int idCounter;
        private bool stop;
        private List<Worker> workers;

        public SimpleServer(int port)
        {
            this.port = port;
            this.stop = false;
            workers = new List<Worker>();

            Thread t = new Thread(Run);
            t.Start();
        }

        private void Run()
        {
            try
            {
                TcpListener server = new TcpListener(IPAddress.Any, port);
                
                server.Start();

                while (!stop)
                    if (server.Pending())
                    {
                        Socket connection = server.AcceptSocket();

                        Worker w = new Worker(this, connection, "Client "+idCounter++);
                        workers.Add(w);
                        Console.WriteLine("Client has connected!");
                    }

                Log("Server is closed!");
            }
            catch (Exception)
            {
                Log("Exception");
            }
        }

        public void Terminate()
        {
            stop = true;

            foreach (Worker w in workers)
                w.Terminate();
        }

        /// <summary>
        /// Sends a message to all connected clients
        /// </summary>
        /// <param name="message">The message to sent to the users</param>
        public void Broadcast(string message, string sender)
        {
            foreach(Worker w in workers)
                w.Send("MESSAGE",true,message, sender);
        }
        
        /// <summary>
        /// Sends a message to all connected clients except the given worker.
        /// </summary>
        /// <param name="message">The message to sent to the users</param>
        /// <param name="worker">The work that does not receive the message</param>
        public void Broadcast(string message, Worker worker)
        {
            foreach (Worker w in workers)
                if(w != worker)
                    w.Send(message);
        }

        public void Remove(Worker worker)
        {
            workers.Remove(worker);
        }

        private void Log(string log)
        {
            Console.WriteLine("[Server]: " + log);
        }
    }
}