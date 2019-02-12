using Shared;
using SharedCode;
using System;
using System.Collections.Concurrent;
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
        private readonly object workersLock = new object();


        public string[] Users {get; set;}

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
                ServerStarted?.Invoke();

                while (!stop)
                    if (server.Pending())
                    {
                        Socket connection = server.AcceptSocket();

                        Worker w = new Worker(this, connection, "Client "+idCounter++);
                        lock (workersLock)
                        {
                            workers.Add(w);
                            UserConnected?.Invoke(w.Username);
                        }
                    }

                server.Server.Close(); // Release all resources
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                ServerClosed?.Invoke();
            }
        }

        public void Terminate()
        {
            lock (workersLock)
            {
                stop = true;

                for (int i=workers.Count-1;i>=0;i--)
                    workers[i].Terminate();
            }
        }

        public bool IsUsernameTaken(string username)
        {
            var workerWithUsername = workers.Where(x => x.Username == username).FirstOrDefault();
            return workerWithUsername != null;
        }

        /// <summary>
        /// Sends a message to all connected clients
        /// </summary>
        /// <param name="message">The message to sent to the users</param>
        public void SendMessage(string message, string sender)
        {
            lock (workersLock)
            {
                foreach (Worker w in workers)
                    w.Send(MessageProtocols.Message, true, message, sender);
                NewMessage?.Invoke(message,sender);
            }
        }

        /// <summary>
        /// Sends a message to all connected clients except the given worker.
        /// </summary>
        /// <param name="message">The message to sent to the users</param>
        /// <param name="worker">The work that does not receive the message</param>
        public void Broadcast(string protocol, params string[] parameters)
        {
            Broadcast(protocol, null, parameters);
        }

        public void Broadcast(string protocol, Worker worker, params string[] parameters)
        {
            lock(workersLock)
                foreach (Worker w in workers)
                    if (worker == null || worker != w)
                        w.Send(protocol, true, parameters);
        }

        public void Remove(Worker worker)
        {
            lock (workersLock)
            {
                workers.Remove(worker);
                UserDisconnected?.Invoke(worker.Username);
            }
        }

        public delegate void OnNewMessage(string message, string sender);
        public event OnNewMessage NewMessage;

        public delegate void OnUserConnected(string user);
        public event OnUserConnected UserConnected;

        public delegate void OnUserDisconnected(string user);
        public event OnUserDisconnected UserDisconnected;

        public delegate void OnServerStarted();
        public event OnServerStarted ServerStarted;

        public delegate void OnServerClosed();
        public event OnServerClosed ServerClosed;


        public List<string> ConnectedUsers()
        {
            lock (workersLock)
            {
                List<string> users = new List<string>();
                workers.ForEach(w => users.Add(w.Username));
                return users;
            }
        }
    }
}