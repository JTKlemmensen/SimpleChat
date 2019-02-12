using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SimpleServer server;
        private bool ServerStarted = false;
        public MainWindow()
        {
            InitializeComponent();
            Server_ServerClosed();
        }

        private void Server_ServerClosed()
        {
            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    ServerToggle.Content = "Start";
                    ServerPort.IsEnabled = true;
                }
            ));
            ServerStarted = false;
        }

        private void Server_ServerStarted()
        {
            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    ServerToggle.Content = "Close";
                    ServerPort.IsEnabled = false;
                }
            ));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(server!=null)
                server.Terminate();
        }

        private void Client_UserDisconnected(string user)
        {
            Dispatcher.BeginInvoke(
                new Action(() => {
                    ListBoxItem item = Users.Items.Cast<ListBoxItem>().FirstOrDefault(l => (string)l.Content == user);
                    Users.Items.Remove(item);
                })
            );
        }
        
        private void Client_UserConnected(string user)
        {
            Dispatcher.BeginInvoke(
                new Action(() => {
                    Users.Items.Add(new ListBoxItem { Content = user });
                })
            );
        }

        private void OnTogleServer(object sender, RoutedEventArgs e)
        {
            if(ServerStarted)
            {
                server.Terminate();
                server = null;

            }
            else
            {
                if (int.TryParse(ServerPort.Text, out int port))
                {
                    server = new SimpleServer(port);
                    server.UserConnected += Client_UserConnected;
                    server.UserDisconnected += Client_UserDisconnected;
                    server.ServerStarted += Server_ServerStarted;
                    server.ServerClosed += Server_ServerClosed;
                    server.UsernameChanged += Server_UsernameChanged;

                    ServerStarted = true;
                }
            }
        }

        private void Server_UsernameChanged(string oldUsername, string changedUsername)
        {
            Dispatcher.BeginInvoke(
                new Action(() => {
                    ListBoxItem item = Users.Items.Cast<ListBoxItem>().FirstOrDefault(l => (string)l.Content == oldUsername);
                    Users.Items.Remove(item);
                    Users.Items.Add(new ListBoxItem { Content = changedUsername });
                })
            );
        }
    }
}
