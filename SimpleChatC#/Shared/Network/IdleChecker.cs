using Shared;
using Shared.Network.Constants;
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
        private double IdleCheckerCooldown = 3;
        private double AllowedSecondsUserIdle = 3;

        public IdleChecker(Connection connection)
        {
            this.connection = connection;
            stopWatch = new Stopwatch();
            Pong();

            Thread t = new Thread(Run);
            t.Start();
        }

        public void Pong()
        {
            HasPonged = true;
            stopWatch.Reset();
            stopWatch.Start();
        }

        private void Run()
        {
            while(!connection.Stop)
                if(HasPonged)
                {
                    if (!IsIdleCheckerOnCooldown())
                        CheckIfUserIsIdle();
                    else
                        Thread.Sleep(1000);
                }
                else
                    if (IsUserIdle())
                        connection.Terminate();
                    else
                        Thread.Sleep(1000);
        }

        private bool IsIdleCheckerOnCooldown()
        {
            return stopWatch.Elapsed.TotalSeconds <= IdleCheckerCooldown;
        }

        private void CheckIfUserIsIdle()
        {
            HasPonged = false;
            connection.Send(MessageProtocols.Ping, true);
            stopWatch.Reset();
            stopWatch.Start();
        }

        private bool IsUserIdle()
        {
            return stopWatch.Elapsed.TotalSeconds > AllowedSecondsUserIdle;
        }
    }
}