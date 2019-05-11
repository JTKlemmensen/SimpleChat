using Shared.Ciphers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Network
{
    public abstract class Connection
    {
        private Socket socket;
        private StreamWriter writer;
        protected SymmetricCipher cipher;
        public bool Stop { get; private set; }
        
        public void Start(Socket socket)
        {
            this.socket = socket;
            Thread t = new Thread(Run);
            t.Start();
        }

        private void Run()
        {
            try
            {
                if (socket == null)
                    return;

                NetworkStream stream = new NetworkStream(socket);
                stream.ReadTimeout = 500;
                using (StreamReader reader = new StreamReader(stream))
                using (writer = new StreamWriter(stream))
                {
                    while (!Stop)
                        try
                        {
                            string line = null;
                            if ((line = reader.ReadLine()) != null)
                                Command(GetNetworkMessage(line));
                        }
                        catch (IOException)
                        {
                            // Read timeout
                        }
                }

            }catch(Exception){ }
            finally
            {
                ConnectionDisconnect?.Invoke();
            }
        }

        public delegate void OnConnectionDisconnect();
        public event OnConnectionDisconnect ConnectionDisconnect;

        public void Send(string protocol, bool encrypt = true, params string[] parameters)
        {
            if (writer == null || writer.BaseStream == null)
                return;
            string line = "";

            if (encrypt && cipher!=null)
            {
                line = NetworkUtil.InsertEscape(cipher.Encrypt(protocol));
                foreach (string s in parameters)
                    line += "," + NetworkUtil.InsertEscape(cipher.Encrypt(s));
            }
            else
            {
                line = NetworkUtil.InsertEscape(protocol);
                foreach (string s in parameters)
                    line += "," + NetworkUtil.InsertEscape(s);
            }

            try
            {
                writer.WriteLine(line);
                writer.Flush();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Formats a message so its easy to work with. Decrypts the message if the cipher is not null.
        /// </summary>
        /// <param name="messageIn">The message that will be converted to an instance of NetworkMessage</param>
        /// <param name="cipher">The SymmetricCipher that will be used for decrypting the message</param>
        /// <returns></returns>
        private NetworkMessage GetNetworkMessage(string messageIn)
        {
            List<string> arguments = NetworkUtil.RemoveEscape(messageIn);
            
            if (cipher != null)
                for (int i = 0; i < arguments.Count(); i++)
                    arguments[i] = cipher.Decrypt(arguments[i]);

            if (arguments.Count() > 0)
            {
                string protocol = arguments[0];
                arguments.RemoveAt(0);
                NetworkMessage message = new NetworkMessage()
                {
                    Protocol = protocol,
                    Arguments = arguments
                };

                return message;
            }

            return null;
        }

        public virtual void Terminate()
        {
            Stop = true;
        }

        protected abstract void Command(NetworkMessage message);
    }
}