using Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class IdleChecker
    {
        private Connection connection;
        private Stopwatch stopWatch;
        private bool HasPinged;
        private double IdleCooldown = 3;
        private double AllowedSecondsIdle = 5;

        public IdleChecker(Connection connection)
        {
            this.connection = connection;
            stopWatch = new Stopwatch();
            HasPinged = true;

            Thread t = new Thread(Run);
        }

        private void Run()
        {
            while(!connection.Stop)
                if(HasPinged)
                {
                    if(!stopWatch.IsRunning)
                        stopWatch.Start();
                    else 
                    {
                        double TimeLeft = IdleCooldown - stopWatch.Elapsed.TotalSeconds;
                        if (TimeLeft <= 0)
                        {
                            HasPinged = false;
                            stopWatch.Reset();
                            stopWatch.Start();

                            //worker.SendMessage("PING");
                            Console.WriteLine("Server sending: PING");
                        }
                        else
                            Thread.Sleep((int)(TimeLeft * 1000));
                    }
                }
                else
                {
                    double TimeLeft = AllowedSecondsIdle - stopWatch.Elapsed.TotalSeconds;
                    if (TimeLeft<=0)
                        connection.Terminate();
                    else
                        Thread.Sleep((int)(TimeLeft * 1000));
                }
        }

        public void Pong()
        {
            HasPinged = true;
            Console.WriteLine("Server received: PONG");
        }
    }
}