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
        public MainWindow()
        {
            InitializeComponent();
            server = new SimpleServer(25565);
            server.UserConnected += Client_UserConnected;
            server.UserDisconnected += Client_UserDisconnected;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
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

        private void OnStopServer(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
