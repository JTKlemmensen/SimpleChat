using Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Network
{
    public class IdleChecker
    {
        private Connection connection;
        private Stopwatch stopWatch;
        private bool HasPonged;
        private double IdleCooldown = 3;
        private double AllowedSecondsIdle = 3;

        public IdleChecker(Connection connection)
        {
            this.connection = connection;
            stopWatch = new Stopwatch();
            Pong();

            Thread t = new Thread(Run);
            t.Start();
        }

        private void Run()
        {
            while(!connection.Stop)
                if(HasPonged)
                {
                    if (stopWatch.Elapsed.TotalSeconds > IdleCooldown)
                    {
                        HasPonged = false;
                        connection.Send(MessageProtocols.Ping, true);
                        stopWatch.Reset();
                        stopWatch.Start();
                    }
                    else
                        Thread.Sleep(1000);
                    
                }
                else
                {
                    if (stopWatch.Elapsed.Seconds > AllowedSecondsIdle)
                        connection.Terminate();
                    else
                        Thread.Sleep(1000);
                }
        }

        public void Pong()
        {
            HasPonged = true;
            stopWatch.Reset();
            stopWatch.Start();
        }
    }
}