using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Worker : Connection
    {
        public string Username { get; private set; }
        private SimpleServer server;

        public Worker(SimpleServer server, Socket connection, string username)
        {
            this.server = server;
            this.Username = username;
            this.Start(connection);
        }

        protected override void Command(NetworkMessage message)
        {
            if (!HasEstablishedConnection())
                EstablishConnection(message);

            switch(message.Protocol)
            {
                case "MESSAGE":
                    if (message.Arguments.Count == 1)
                        server.SendMessage(message.Arguments[0], Username);
                    break;

                case "END":
                    Terminate();
                    break;

                default:
                    Console.WriteLine(message.Protocol);
                    break;
            }
        }

        private bool HasEstablishedConnection()
        {
            return cipher != null;
        }

        private void EstablishConnection(NetworkMessage message)
        {
            //Generate symmetric key + IV, save it and encrypt it with the given public assymetric key and send it to the client.   
            try
            {
                AsymmetricCipher asymmetric = new AsymmetricCipher();
                asymmetric.LoadPublicKey(message.Protocol);   
                cipher = new SymmetricCipher();

                Send(asymmetric.Encrypt(cipher.Key), false, asymmetric.Encrypt(cipher.IV));
                server.Broadcast("CONNECT", this, Username); // Tell users that it connected
                Send("USERS", true, server.ConnectedUsers().ToArray());
            }
            catch (Exception)
            {
                //Terminate();
            }
        }

        public override void Terminate()
        {
            server.Broadcast("DISCONNECT", this, Username);
            base.Terminate();
            server.Remove(this);
        }
    }
}