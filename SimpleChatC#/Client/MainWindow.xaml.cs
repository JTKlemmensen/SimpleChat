using Client;
using Newtonsoft.Json;
using Shared;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool IsConnected;
        private SimpleClient client;
        public MainWindow()
        {
            InitializeComponent();
            DisconnectedToServer();
        }

        private void Client_UserDisconnected(string user)
        {
            Dispatcher.BeginInvoke(
                new Action(() => {
                    MessagesArea.Text += "\n" + user + " has disconnected";
                    ListBoxItem item = Users.Items.Cast<ListBoxItem>().FirstOrDefault(l => (string)l.Content == user);
                    Users.Items.Remove(item);
                })
            );
        }

        private void Client_TotalUsers(IEnumerable<string> users)
        {
            Dispatcher.BeginInvoke(
                new Action(() => {
                    foreach(string user in users)
                        Users.Items.Add(new ListBoxItem { Content = user });
                })
            );
        }

        private void Client_UserConnected(string user)
        {
            Console.WriteLine("CONNECTED TO A SERER!");
            Dispatcher.BeginInvoke(
                new Action(() => {
                    MessagesArea.Text += "\n" + user + " has connected";
                    Users.Items.Add(new ListBoxItem { Content = user });
                })
            );
        }

        private void OnNewMessage(string message, string sender)
        {
            Dispatcher.BeginInvoke(
                new Action(() => {
                    MessagesArea.Text += "\n["+sender+"]: "+message;
                })
            );
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            if(client!=null)
                client.SendMessage(Message.Text);
            Message.Text = "";
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(client!=null)
                client.Terminate();
        }

        private void ConnectedToServer()
        {
            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    IsConnected = true;
                    ToggleConnection.Content = "Disconnect";
                    ServerIP.IsEnabled = false;
                    ServerPort.IsEnabled = false;
                }
            ));
        }

        private void DisconnectedToServer()
        {
            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    IsConnected = false;
                    ToggleConnection.Content = "Connect";
                    ServerIP.IsEnabled = true;
                    ServerPort.IsEnabled = true;
                    MessagesArea.Text = "";
                    Users.Items.Clear();
                }
            ));
        }

        private void OnToggleConnection(object sender, RoutedEventArgs e)
        {
            Console.WriteLine((client == null) +" "+ IsConnected);
            if(IsConnected)
            {
                if (client != null)
                {
                    client.Terminate();
                    client = null;
                }
            }
            else
            {
                if (int.TryParse(ServerPort.Text, out int port))
                {
                    client = new SimpleClient(ServerIP.Text, port);
                    client.NewMessage += OnNewMessage;
                    client.UserConnected += Client_UserConnected;
                    client.TotalUsers += Client_TotalUsers;
                    client.UserDisconnected += Client_UserDisconnected;

                    client.ConnectionConnect += ConnectedToServer;
                    client.ConnectionDisconnect += DisconnectedToServer;
                }
            }
        }
    }
}
