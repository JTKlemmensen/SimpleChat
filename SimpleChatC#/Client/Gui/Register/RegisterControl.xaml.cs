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

namespace SimpleChat.Gui.Register
{
    /// <summary>
    /// Interaction logic for RegisterControl.xaml
    /// </summary>
    public partial class RegisterControl : UserControl
    {
        private IWindowController controller;

        public RegisterControl(IWindowController controller)
        {
            InitializeComponent();
            this.controller = controller;
        }

        private void OnGotoLogin(object sender, MouseButtonEventArgs e)
        {
            controller.GotoLogin();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            controller.TerminateConnection();
        }

        private void Register(object sender, RoutedEventArgs e)
        {
            controller.ChatClient.Register(username.Text, password.Password);
        }
    }
}