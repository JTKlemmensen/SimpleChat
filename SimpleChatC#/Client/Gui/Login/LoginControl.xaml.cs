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

namespace SimpleChat.Gui.Login
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        private IWindowController controller;

        public LoginControl(IWindowController controller)
        {
            InitializeComponent();
            this.controller = controller;
        }

        private void OnGotoRegisterAccount(object sender, MouseButtonEventArgs e)
        {
            controller.GotoRegister();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            controller.TerminateConnection();
        }

        private void Login(object sender, RoutedEventArgs e)
        {
            controller.ChatClient.Login(username.Text, password.Password);
        }
    }
}